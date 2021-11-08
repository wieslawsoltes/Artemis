﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.UI.Avalonia.Shared;
using DynamicData;
using ReactiveUI;

namespace Artemis.UI.Avalonia.Screens.Plugins.ViewModels
{
    public class PluginPrerequisiteViewModel : ActivatableViewModelBase
    {
        private readonly bool _uninstall;
        private readonly ObservableAsPropertyHelper<bool> _busy;
        private readonly ObservableAsPropertyHelper<int> _activeStepNumber;

        private bool _installing;
        private bool _uninstalling;
        private bool _isMet;

        private PluginPrerequisiteActionViewModel? _activeAction;

        public PluginPrerequisiteViewModel(PluginPrerequisite pluginPrerequisite, bool uninstall)
        {
            _uninstall = uninstall;

            PluginPrerequisite = pluginPrerequisite;
            Actions = new ObservableCollection<PluginPrerequisiteActionViewModel>(!_uninstall
                ? PluginPrerequisite.InstallActions.Select(a => new PluginPrerequisiteActionViewModel(a))
                : PluginPrerequisite.UninstallActions.Select(a => new PluginPrerequisiteActionViewModel(a)));

            this.WhenAnyValue(x => x.Installing, x => x.Uninstalling, (i, u) => i || u).ToProperty(this, x => x.Busy, out _busy);
            this.WhenAnyValue(x => x.ActiveAction, a => Actions.IndexOf(a)).ToProperty(this, x => x.ActiveStepNumber, out _activeStepNumber);

            PluginPrerequisite.PropertyChanged += PluginPrerequisiteOnPropertyChanged;

            // Could be slow so take it off of the UI thread
            Task.Run(() => IsMet = PluginPrerequisite.IsMet());
        }

        public ObservableCollection<PluginPrerequisiteActionViewModel> Actions { get; }

        public PluginPrerequisiteActionViewModel? ActiveAction
        {
            get => _activeAction;
            set => this.RaiseAndSetIfChanged(ref _activeAction , value);
        }

        public PluginPrerequisite PluginPrerequisite { get; }

        public bool Installing
        {
            get => _installing;
            set => this.RaiseAndSetIfChanged(ref _installing, value);
        }

        public bool Uninstalling
        {
            get => _uninstalling;
            set => this.RaiseAndSetIfChanged(ref _uninstalling, value);
        }

        public bool IsMet
        {
            get => _isMet;
            set => this.RaiseAndSetIfChanged(ref _isMet, value);
        }

        public bool Busy => _busy.Value;
        public int ActiveStepNumber => _activeStepNumber.Value;
        public bool HasMultipleActions => Actions.Count > 1;

        public async Task Install(CancellationToken cancellationToken)
        {
            if (Busy)
                return;

            Installing = true;
            try
            {
                await PluginPrerequisite.Install(cancellationToken);
            }
            finally
            {
                Installing = false;
                IsMet = PluginPrerequisite.IsMet();
            }
        }

        public async Task Uninstall(CancellationToken cancellationToken)
        {
            if (Busy)
                return;

            Uninstalling = true;
            try
            {
                await PluginPrerequisite.Uninstall(cancellationToken);
            }
            finally
            {
                Uninstalling = false;
                IsMet = PluginPrerequisite.IsMet();
            }
        }

        private void PluginPrerequisiteOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PluginPrerequisite.CurrentAction))
                ActivateCurrentAction();
        }

        private void ActivateCurrentAction()
        {
            PluginPrerequisiteActionViewModel? activeAction = Actions.FirstOrDefault(i => i.Action == PluginPrerequisite.CurrentAction);
            if (activeAction == null)
                return;

            ActiveAction = activeAction;
        }

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PluginPrerequisite.PropertyChanged -= PluginPrerequisiteOnPropertyChanged;
            }

            base.Dispose(disposing);
        }

        #endregion

    }
}