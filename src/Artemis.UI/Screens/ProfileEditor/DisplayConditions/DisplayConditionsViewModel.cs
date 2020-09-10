﻿using System.Linq;
using Artemis.Core;
using Artemis.UI.Ninject.Factories;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using Stylet;

namespace Artemis.UI.Screens.ProfileEditor.DisplayConditions
{
    public class DisplayConditionsViewModel : Conductor<DisplayConditionGroupViewModel>, IProfileEditorPanelViewModel
    {
        private readonly IDisplayConditionsVmFactory _displayConditionsVmFactory;
        private readonly IProfileEditorService _profileEditorService;
        private bool _alwaysFinishTimeline;
        private bool _displayContinuously;
        private RenderProfileElement _renderProfileElement;
        private int _transitionerIndex;

        public DisplayConditionsViewModel(IProfileEditorService profileEditorService, IDisplayConditionsVmFactory displayConditionsVmFactory)
        {
            _profileEditorService = profileEditorService;
            _displayConditionsVmFactory = displayConditionsVmFactory;
        }

        public int TransitionerIndex
        {
            get => _transitionerIndex;
            set => SetAndNotify(ref _transitionerIndex, value);
        }


        public RenderProfileElement RenderProfileElement
        {
            get => _renderProfileElement;
            set => SetAndNotify(ref _renderProfileElement, value);
        }

        public bool DisplayContinuously
        {
            get => _displayContinuously;
            set
            {
                if (!SetAndNotify(ref _displayContinuously, value)) return;
                _profileEditorService.UpdateSelectedProfileElement();
            }
        }

        public bool AlwaysFinishTimeline
        {
            get => _alwaysFinishTimeline;
            set
            {
                if (!SetAndNotify(ref _alwaysFinishTimeline, value)) return;
                _profileEditorService.UpdateSelectedProfileElement();
            }
        }

        public bool ConditionBehaviourEnabled => RenderProfileElement != null;

        protected override void OnActivate()
        {
            _profileEditorService.ProfileElementSelected += ProfileEditorServiceOnProfileElementSelected;
        }

        protected override void OnDeactivate()
        {
            _profileEditorService.ProfileElementSelected -= ProfileEditorServiceOnProfileElementSelected;
        }

        private void ProfileEditorServiceOnProfileElementSelected(object sender, RenderProfileElementEventArgs e)
        {
            RenderProfileElement = e.RenderProfileElement;
            NotifyOfPropertyChange(nameof(ConditionBehaviourEnabled));

            _displayContinuously = RenderProfileElement?.DisplayContinuously ?? false;
            NotifyOfPropertyChange(nameof(DisplayContinuously));
            _alwaysFinishTimeline = RenderProfileElement?.AlwaysFinishTimeline ?? false;
            NotifyOfPropertyChange(nameof(AlwaysFinishTimeline));

            if (e.RenderProfileElement == null)
            {
                ActiveItem = null;
                return;
            }

            // Ensure the layer has a root display condition group
            if (e.RenderProfileElement.DisplayConditionGroup == null)
                e.RenderProfileElement.DisplayConditionGroup = new DisplayConditionGroup(null);

            ActiveItem = _displayConditionsVmFactory.DisplayConditionGroupViewModel(e.RenderProfileElement.DisplayConditionGroup, false);
            ActiveItem.IsRootGroup = true;
            ActiveItem.Update();

            // Only show the intro to conditions once, and only if the layer has no conditions
            if (TransitionerIndex != 1)
                TransitionerIndex = ActiveItem.Items.Any() ? 1 : 0;
        }
    }
}