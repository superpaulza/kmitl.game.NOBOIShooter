using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace NOBOIShooter.Controls
{
    /// <summary>
    /// Unifies Mouse (Windows/Desktop) + Touch (Android/iOS) into one pointer.
    /// All positions are returned in VIRTUAL (1280x720 design) coordinates
    /// so gameplay/hit-tests work on any screen size/aspect.
    /// </summary>
    public static class InputHelper
    {
        private static MouseState _prevMouse;
        private static MouseState _currMouse;
        private static bool _prevPressed;
        private static bool _currPressed;
        private static Vector2 _currVirtualPos = Vector2.Zero;
        private static Vector2 _prevVirtualPos = Vector2.Zero;
        private static bool _initialized;

        public static Vector2 Position => _currVirtualPos;
        public static Vector2 PreviousPosition => _prevVirtualPos;

        // Right-button state (desktop only) for ball-swap.
        private static MouseState _prevMouseRight;
        private static MouseState _currMouseRightState;

        public static void Update()
        {
            _prevVirtualPos = _currVirtualPos;
            _prevPressed = _currPressed;
            _prevMouse = _currMouse;

            _currMouse = Mouse.GetState();
            bool mousePressed = _currMouse.LeftButton == ButtonState.Pressed;
            Vector2 mouseVirtual = Singleton.Instance.ToVirtual(new Vector2(_currMouse.X, _currMouse.Y));

            // Touch takes precedence when touching (mobile).
            TouchCollection touches = TouchPanel.GetState();
            bool touchActive = touches.Count > 0;
            bool touchPressed = false;
            Vector2 touchVirtual = _currVirtualPos;

            if (touchActive)
            {
                // Use first touch for buttons/aiming.
                TouchLocation t = touches[0];
                touchVirtual = Singleton.Instance.ToVirtual(t.Position);
                touchPressed = t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved;
            }

            if (touchActive)
            {
                _currPressed = touchPressed;
                _currVirtualPos = touchVirtual;
            }
            else
            {
                _currPressed = mousePressed;
                // On desktop the mouse always has a position even when not pressed.
                // On mobile Mouse.GetState() sits at (0,0) — ignore it unless the
                // mouse actually moved or a mouse button is down, otherwise keep
                // the last touch position (critical on the tap-release frame so
                // buttons/shots still hit-test at the finger location).
                if (!_initialized || _currMouse.X != _prevMouse.X || _currMouse.Y != _prevMouse.Y || mousePressed)
                    _currVirtualPos = mouseVirtual;
            }

            _prevMouseRight = _currMouseRightState;
            _currMouseRightState = _currMouse;
            _initialized = true;
        }

        /// <summary>True on the frame the pointer went down (tap / left-click down).</summary>
        public static bool IsPressed() => _currPressed && !_prevPressed;

        /// <summary>True on the frame the pointer was released (tap / click release).</summary>
        public static bool IsReleased() => !_currPressed && _prevPressed;

        /// <summary>True while pointer is held down.</summary>
        public static bool IsDown() => _currPressed;

        /// <summary>Click semantics used by buttons: released inside after press.</summary>
        /// <param name="wasHovering">whether pointer is currently inside the control rect.</param>
        public static bool ClickedThisFrame(bool wasHovering)
        {
            return wasHovering && IsReleased();
        }

        /// <summary>Desktop right-click edge (ball swap). No-op on touch.</summary>
        public static bool IsRightClicked()
        {
            return _currMouseRightState.RightButton == ButtonState.Pressed
                && _prevMouseRight.RightButton == ButtonState.Released;
        }
    }
}
