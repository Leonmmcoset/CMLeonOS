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

namespace CMLeonOS.UILib.Animations
{
    /// <summary>
    /// A window animation.
    /// </summary>
    internal abstract class Animation
    {
        /// <summary>
        /// The easing type of the animation.
        /// </summary>
        internal EasingType EasingType { get; set; } = EasingType.Sine;

        /// <summary>
        /// The direction of the easing of the animation.
        /// </summary>
        internal EasingDirection EasingDirection { get; set; } = EasingDirection.Out;

        /// <summary>
        /// The duration of the animation.
        /// </summary>
        internal int Duration { get; set; } = 60;

        /// <summary>
        /// How many frames of the animation have been completed.
        /// </summary>
        internal int Position { get; set; } = 0;

        /// <summary>
        /// If the animation has finished.
        /// </summary>
        internal bool Finished
        {
            get
            {
                return Position >= Duration;
            }
        }

        /// <summary>
        /// The window associated with the animation.
        /// </summary>
        internal Window Window { get; set; }

        /// <summary>
        /// Advance the animation by one frame.
        /// </summary>
        /// <returns>Whether or not the animation is now finished.</returns>
        internal abstract bool Advance();

        private int? timerId { get; set; } = null;

        /// <summary>
        /// Start the animation.
        /// </summary>
        internal void Start()
        {
            if (timerId == null)
            {
                timerId = Cosmos.HAL.Global.PIT.RegisterTimer(new Cosmos.HAL.PIT.PITTimer(() =>
                {
                    Advance();
                    if (Finished)
                    {
                        Stop();
                    }
                }, (ulong)((1000d /* ms */ / 60d) * 1e+6d /* ms -> ns */ ), true));
            }
        }

        /// <summary>
        /// Stop the animation.
        /// </summary>
        internal void Stop()
        {
            if (timerId != null)
            {
                Cosmos.HAL.Global.PIT.UnregisterTimer((int)timerId);
                timerId = null;
            }
        }
    }
}
