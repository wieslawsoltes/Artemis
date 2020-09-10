﻿using System;
using System.Collections.Generic;
using Artemis.Core.Services;
using Artemis.Storage.Entities.Profile.Abstract;

namespace Artemis.Core
{
    /// <summary>
    ///     An abstract class for display condition parts
    /// </summary>
    public abstract class DisplayConditionPart : IDisposable
    {
        private readonly List<DisplayConditionPart> _children = new List<DisplayConditionPart>();

        /// <summary>
        ///     Gets the parent of this part
        /// </summary>
        public DisplayConditionPart Parent { get; internal set; }

        /// <summary>
        ///     Gets the children of this part
        /// </summary>
        public IReadOnlyList<DisplayConditionPart> Children => _children.AsReadOnly();

        /// <summary>
        ///     Adds a child to the display condition part's <see cref="Children" /> collection
        /// </summary>
        /// <param name="displayConditionPart"></param>
        public void AddChild(DisplayConditionPart displayConditionPart)
        {
            if (!_children.Contains(displayConditionPart))
            {
                displayConditionPart.Parent = this;
                _children.Add(displayConditionPart);
            }
        }

        /// <summary>
        ///     Removes a child from the display condition part's <see cref="Children" /> collection
        /// </summary>
        /// <param name="displayConditionPart">The child to remove</param>
        public void RemoveChild(DisplayConditionPart displayConditionPart)
        {
            if (_children.Contains(displayConditionPart))
            {
                displayConditionPart.Parent = null;
                _children.Remove(displayConditionPart);
            }
        }

        /// <summary>
        ///     Evaluates the condition part on the data model
        /// </summary>
        /// <returns></returns>
        public abstract bool Evaluate();

        /// <summary>
        ///     Evaluates the condition part on the given target (currently only for lists)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public abstract bool EvaluateObject(object target);

        internal abstract void Save();
        internal abstract DisplayConditionPartEntity GetEntity();

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}