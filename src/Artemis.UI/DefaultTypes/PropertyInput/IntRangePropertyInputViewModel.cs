﻿using System;
using Artemis.Core;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using FluentValidation;
using Stylet;

namespace Artemis.UI.DefaultTypes.PropertyInput
{
    public class IntRangePropertyInputViewModel : PropertyInputViewModel<IntRange>
    {
        public IntRangePropertyInputViewModel(LayerProperty<IntRange> layerProperty,
            IProfileEditorService profileEditorService,
            IModelValidator<IntRangePropertyInputViewModel> validator) : base(layerProperty, profileEditorService, validator)
        {
        }

        public int Start
        {
            get => InputValue?.Start ?? 0;
            set
            {
                if (InputValue == null)
                    InputValue = new IntRange(value, value + 1);
                else
                    InputValue.Start = value;

                NotifyOfPropertyChange(nameof(Start));
            }
        }

        public int End
        {
            get => InputValue?.End ?? 0;
            set
            {
                if (InputValue == null)
                    InputValue = new IntRange(value - 1, value);
                else
                    InputValue.End = value;

                NotifyOfPropertyChange(nameof(End));
            }
        }


        protected override void OnInputValueChanged()
        {
            NotifyOfPropertyChange(nameof(Start));
            NotifyOfPropertyChange(nameof(End));
        }
    }

    public class IntRangePropertyInputViewModelValidator : AbstractValidator<IntRangePropertyInputViewModel>
    {
        public IntRangePropertyInputViewModelValidator()
        {
            RuleFor(vm => vm.Start).LessThanOrEqualTo(vm => vm.End);
            RuleFor(vm => vm.End).GreaterThanOrEqualTo(vm => vm.Start);

            RuleFor(vm => vm.Start)
                .GreaterThanOrEqualTo(vm => Convert.ToInt32(vm.LayerProperty.PropertyDescription.MinInputValue))
                .When(vm => vm.LayerProperty.PropertyDescription.MinInputValue.IsNumber());
            RuleFor(vm => vm.End)
                .LessThanOrEqualTo(vm => Convert.ToInt32(vm.LayerProperty.PropertyDescription.MaxInputValue))
                .When(vm => vm.LayerProperty.PropertyDescription.MaxInputValue.IsNumber());
        }
    }
}