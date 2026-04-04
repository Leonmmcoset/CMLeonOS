// The CMLeonOS Project (https://github.com/Leonmmcoset/CMLeonOS)
// Copyright (C) 2025-present LeonOS 2 Developer Team
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using CMLeonOS.Gui;
using CMLeonOS.Gui.UILib;
using System;
using System.Drawing;

namespace CMLeonOS.UILib.Animations
{
    /// <summary>
    /// An animation that moves or resizes a window.
    /// </summary>
    internal class MovementAnimation : Animation
    {
        /// <summary>
        /// Initialise the animation.
        /// </summary>
        /// <param name="window">The window associated with the animation.</param>
        /// <param name="to">The goal of the animation.</param>
        internal MovementAnimation(Window window)
        {
            Window = window;
            From = new Rectangle(window.X, window.Y, window.Width, window.Height);
        }

        /// <summary>
        /// The starting rectangle of the animation.
        /// </summary>
        internal Rectangle From;

        /// <summary>
        /// The goal rectangle of the animation. 
        /// </summary>
        internal Rectangle To;

        internal override bool Advance()
        {
            if (From.IsEmpty || To.IsEmpty) throw new Exception("The From or To value of this MovementAnimation is empty.");
            Position++;
            if (Position == Duration)
            {
                Window.MoveAndResize(To.X, To.Y, To.Width, To.Height);
                if (Window is Control control)
                {
                    control.Render();
                }
                Advanced?.Invoke();
            }
            else
            {
                double t = Easing.Ease(Position / (double)Duration, EasingType, EasingDirection);
                Rectangle current = new Rectangle(
                    (int)Easing.Lerp(From.X, To.X, t),
                    (int)Easing.Lerp(From.Y, To.Y, t),
                    (int)Easing.Lerp(From.Width, To.Width, t),
                    (int)Easing.Lerp(From.Height, To.Height, t)
                );
                Window.MoveAndResize(current.X, current.Y, current.Width, current.Height);
                if (Window is Control control)
                {
                    control.Render();
                }
                Advanced?.Invoke();
            }
            return Finished;
        }
    }
}
