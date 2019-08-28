using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Input;
using Point = System.Drawing.Point;

namespace TLib.Windows
{
    /// <summary>
    /// Exposes a simple interface to common mouse operations, allowing the user to simulate mouse input.
    /// </summary>
    /// <example>The following code moves to screen coordinate 100,100 and left clicks.
    /// <code>
    /**
        Mouse.MoveTo(new Point(100, 100));
        Mouse.Click(MouseButton.Left);
    */
    /// </code>
    /// </example>
    public static class MouseSimulation
    {
        /// <summary>
        /// Clicks a mouse button.
        /// </summary>
        /// <param name="mouseButton">The mouse button to click.</param>
        public static void Click(MouseButton mouseButton)
        {
            Down(mouseButton);
            Up(mouseButton);
        }

        /// <summary>
        /// Double-clicks a mouse button.
        /// </summary>
        /// <param name="mouseButton">The mouse button to click.</param>
        public static void DoubleClick(MouseButton mouseButton)
        {
            Click(mouseButton);
            Click(mouseButton);
        }

        /// <summary>
        /// Performs a mouse-down operation for a specified mouse button.
        /// </summary>
        /// <param name="mouseButton">The mouse button to use.</param>
        public static void Down(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.LeftDown);
                    break;
                case MouseButton.Right:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.RightDown);
                    break;
                case MouseButton.Middle:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.MiddleDown);
                    break;
                case MouseButton.XButton1:
                    SendMouseInput(0, 0, NativeMethods.XButton1, NativeMethods.SendMouseInputFlags.XDown);
                    break;
                case MouseButton.XButton2:
                    SendMouseInput(0, 0, NativeMethods.XButton2, NativeMethods.SendMouseInputFlags.XDown);
                    break;
                default:
#pragma warning disable CA1303 // 请不要将文本作为本地化参数传递
                    throw new InvalidOperationException("Unsupported MouseButton input.");
#pragma warning restore CA1303 // 请不要将文本作为本地化参数传递
            }
        }

        /// <summary>
        /// Moves the mouse pointer to the specified screen coordinates.
        /// </summary>
        /// <param name="point">The screen coordinates to move to.</param>
        public static void MoveTo(Point point)
        {
            SendMouseInput(point.X, point.Y, 0, NativeMethods.SendMouseInputFlags.Move | NativeMethods.SendMouseInputFlags.Absolute);
        }

        /// <summary>
        /// Resets the system mouse to a clean state.
        /// </summary>
        public static void Reset()
        {
            MoveTo(new Point(0, 0));

            if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {
                SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.LeftUp);
            }

            if (System.Windows.Input.Mouse.MiddleButton == MouseButtonState.Pressed)
            {
                SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.MiddleUp);
            }

            if (System.Windows.Input.Mouse.RightButton == MouseButtonState.Pressed)
            {
                SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.RightUp);
            }

            if (System.Windows.Input.Mouse.XButton1 == MouseButtonState.Pressed)
            {
                SendMouseInput(0, 0, NativeMethods.XButton1, NativeMethods.SendMouseInputFlags.XUp);
            }

            if (System.Windows.Input.Mouse.XButton2 == MouseButtonState.Pressed)
            {
                SendMouseInput(0, 0, NativeMethods.XButton2, NativeMethods.SendMouseInputFlags.XUp);
            }
        }

        /// <summary>
        /// Simulates scrolling of the mouse wheel up or down.
        /// </summary>
        /// <param name="lines">The number of lines to scroll. Use positive numbers to scroll up and negative numbers to scroll down.</param>
        public static void Scroll(double lines)
        {
            int amount = (int)(NativeMethods.WheelDelta * lines);

            SendMouseInput(0, 0, amount, NativeMethods.SendMouseInputFlags.Wheel);
        }

        /// <summary>
        /// Performs a mouse-up operation for a specified mouse button.
        /// </summary>
        /// <param name="mouseButton">The mouse button to use.</param>
        public static void Up(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.LeftUp);
                    break;
                case MouseButton.Right:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.RightUp);
                    break;
                case MouseButton.Middle:
                    SendMouseInput(0, 0, 0, NativeMethods.SendMouseInputFlags.MiddleUp);
                    break;
                case MouseButton.XButton1:
                    SendMouseInput(0, 0, NativeMethods.XButton1, NativeMethods.SendMouseInputFlags.XUp);
                    break;
                case MouseButton.XButton2:
                    SendMouseInput(0, 0, NativeMethods.XButton2, NativeMethods.SendMouseInputFlags.XUp);
                    break;
                default:
#pragma warning disable CA1303 // 请不要将文本作为本地化参数传递
                    throw new InvalidOperationException("Unsupported MouseButton input.");
#pragma warning restore CA1303 // 请不要将文本作为本地化参数传递
            }
        }

        /// <summary>
        /// Sends mouse input.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="data">scroll wheel amount</param>
        /// <param name="flags">SendMouseInputFlags flags</param>
        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        private static void SendMouseInput(int x, int y, int data, NativeMethods.SendMouseInputFlags flags)
        {
            PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);
            permissions.Demand();

            int intflags = (int)flags;

            if ((intflags & (int)NativeMethods.SendMouseInputFlags.Absolute) != 0)
            {
                // Absolute position requires normalized coordinates.
                NormalizeCoordinates(ref x, ref y);
                intflags |= NativeMethods.MouseeventfVirtualdesk;
            }

            NativeMethods.INPUT mi = new NativeMethods.INPUT
            {
                type = NativeMethods.InputMouse
            };
            mi.union.mouseInput.dx = x;
            mi.union.mouseInput.dy = y;
            mi.union.mouseInput.mouseData = data;
            mi.union.mouseInput.dwFlags = intflags;
            mi.union.mouseInput.time = 0;
            mi.union.mouseInput.dwExtraInfo = new IntPtr(0);

            if (NativeMethods.SendInput(1, ref mi, Marshal.SizeOf(mi)) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static void NormalizeCoordinates(ref int x, ref int y)
        {
            int vScreenWidth = NativeMethods.GetSystemMetrics(NativeMethods.SMCxvirtualscreen);
            int vScreenHeight = NativeMethods.GetSystemMetrics(NativeMethods.SMCyvirtualscreen);
            int vScreenLeft = NativeMethods.GetSystemMetrics(NativeMethods.SMXvirtualscreen);
            int vScreenTop = NativeMethods.GetSystemMetrics(NativeMethods.SMYvirtualscreen);

            // Absolute input requires that input is in 'normalized' coords - with the entire
            // desktop being (0,0)...(65536,65536). Need to convert input x,y coords to this
            // first.
            //
            // In this normalized world, any pixel on the screen corresponds to a block of values
            // of normalized coords - eg. on a 1024x768 screen,
            // y pixel 0 corresponds to range 0 to 85.333,
            // y pixel 1 corresponds to range 85.333 to 170.666,
            // y pixel 2 correpsonds to range 170.666 to 256 - and so on.
            // Doing basic scaling math - (x-top)*65536/Width - gets us the start of the range.
            // However, because int math is used, this can end up being rounded into the wrong
            // pixel. For example, if we wanted pixel 1, we'd get 85.333, but that comes out as
            // 85 as an int, which falls into pixel 0's range - and that's where the pointer goes.
            // To avoid this, we add on half-a-"screen pixel"'s worth of normalized coords - to
            // push us into the middle of any given pixel's range - that's the 65536/(Width*2)
            // part of the formula. So now pixel 1 maps to 85+42 = 127 - which is comfortably
            // in the middle of that pixel's block.
            // The key ting here is that unlike points in coordinate geometry, pixels take up
            // space, so are often better treated like rectangles - and if you want to target
            // a particular pixel, target its rectangle's midpoint, not its edge.
            x = ((x - vScreenLeft) * 65536) / vScreenWidth + 65536 / (vScreenWidth * 2);
            y = ((y - vScreenTop) * 65536) / vScreenHeight + 65536 / (vScreenHeight * 2);
        }
    }

    /// <summary>
    /// Exposes a simple interface to common keyboard operations, allowing the user to simulate keyboard input.
    /// </summary>
    /// <example>
    /// The following code types "Hello world" with the specified casing,
    /// and then types "hello, capitalized world" which will be in all caps because
    /// the left shift key is being held down.
    /// <code>
    /**
            Keyboard.Type("Hello world");
            Keyboard.Press(Key.LeftShift);
            Keyboard.Type("hello, capitalized world");
            Keyboard.Release(Key.LeftShift);
    */
    /// </code>
    /// </example>
    public static class KeyboardSimulation
    {
        #region Public Members

        /// <summary>
        /// Presses down a key.
        /// </summary>
        /// <param name="key">The key to press.</param>
        public static void Press(Key key)
        {
            SendKeyboardInput(key, true);
        }

        /// <summary>
        /// Releases a key.
        /// </summary>
        /// <param name="key">The key to release.</param>
        public static void Release(Key key)
        {
            SendKeyboardInput(key, false);
        }

        /// <summary>
        /// Resets the system keyboard to a clean state.
        /// </summary>
        public static void Reset()
        {
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                if (key != Key.None && (Keyboard.GetKeyStates(key) & KeyStates.Down) > 0)
                {
                    Release(key);
                }
            }
        }

        /// <summary>
        /// Performs a press-and-release operation for the specified key, which is effectively equivallent to typing.
        /// </summary>
        /// <param name="key">The key to press.</param>
        public static void Type(Key key)
        {
            Press(key);
            Release(key);
        }

        /// <summary>
        /// Types the specified text.
        /// </summary>
        /// <param name="text">The text to type.</param>
        public static void Type(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            foreach (char c in text)
            {
                // We get the vKey value for the character via a Win32 API. We then use bit masks to pull the
                // upper and lower bytes to get the shift state and key information. We then use WPF KeyInterop
                // to go from the vKey key info into a System.Windows.Input.Key data structure. This work is
                // necessary because Key doesn't distinguish between upper and lower case, so we have to wrap
                // the key type inside a shift press/release if necessary.
                int vKeyValue = NativeMethods.VkKeyScan(c);
                bool keyIsShifted = (vKeyValue & NativeMethods.VKeyShiftMask) == NativeMethods.VKeyShiftMask;
                Key key = KeyInterop.KeyFromVirtualKey(vKeyValue & NativeMethods.VKeyCharMask);

                if (keyIsShifted)
                {
                    Type(key, new Key[] { Key.LeftShift });
                }
                else
                {
                    Type(key);
                }
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Types a key while a set of modifier keys are being pressed. Modifer keys
        /// are pressed in the order specified and released in reverse order.
        /// </summary>
        /// <param name="key">Key to type.</param>
        /// <param name="modifierKeys">Set of keys to hold down with key is typed.</param>
        private static void Type(Key key, Key[] modifierKeys)
        {
            foreach (Key modiferKey in modifierKeys)
            {
                Press(modiferKey);
            }

            Type(key);

            foreach (Key modifierKey in modifierKeys.Reverse())
            {
                Release(modifierKey);
            }
        }

        /// <summary>
        /// Injects keyboard input into the system.
        /// </summary>
        /// <param name="key">Indicates the key pressed or released. Can be one of the constants defined in the Key enum.</param>
        /// <param name="press">True to inject a key press, false to inject a key release.</param>
        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        private static void SendKeyboardInput(Key key, bool press)
        {
            PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);
            permissions.Demand();

            NativeMethods.INPUT ki = new NativeMethods.INPUT
            {
                type = NativeMethods.InputKeyboard
            };
            ki.union.keyboardInput.wVk = (short)KeyInterop.VirtualKeyFromKey(key);
            ki.union.keyboardInput.wScan = (short)NativeMethods.MapVirtualKey(ki.union.keyboardInput.wVk, 0);

            int dwFlags = 0;

            if (ki.union.keyboardInput.wScan > 0)
            {
                dwFlags |= NativeMethods.KeyeventfScancode;
            }

            if (!press)
            {
                dwFlags |= NativeMethods.KeyeventfKeyup;
            }

            ki.union.keyboardInput.dwFlags = dwFlags;

            if (ExtendedKeys.Contains(key))
            {
                ki.union.keyboardInput.dwFlags |= NativeMethods.KeyeventfExtendedkey;
            }

            ki.union.keyboardInput.time = 0;
            ki.union.keyboardInput.dwExtraInfo = new IntPtr(0);

            if (NativeMethods.SendInput(1, ref ki, Marshal.SizeOf(ki)) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        // From the SDK:
        // The extended-key flag indicates whether the keystroke message originated from one of
        // the additional keys on the enhanced keyboard. The extended keys consist of the ALT and
        // CTRL keys on the right-hand side of the keyboard; the INS, DEL, HOME, END, PAGE UP,
        // PAGE DOWN, and arrow keys in the clusters to the left of the numeric keypad; the NUM LOCK
        // key; the BREAK (CTRL+PAUSE) key; the PRINT SCRN key; and the divide (/) and ENTER keys in
        // the numeric keypad. The extended-key flag is set if the key is an extended key. 
        //
        // - docs appear to be incorrect. Use of Spy++ indicates that break is not an extended key.
        // Also, menu key and windows keys also appear to be extended.
        private static readonly Key[] ExtendedKeys = new Key[] {
                                                                   Key.RightAlt,
                                                                   Key.RightCtrl,
                                                                   Key.NumLock,
                                                                   Key.Insert,
                                                                   Key.Delete,
                                                                   Key.Home,
                                                                   Key.End,
                                                                   Key.Prior,
                                                                   Key.Next,
                                                                   Key.Up,
                                                                   Key.Down,
                                                                   Key.Left,
                                                                   Key.Right,
                                                                   Key.Apps,
                                                                   Key.RWin,
                                                                   Key.LWin,Key.VolumeUp };
        // Note that there are no distinct values for the following keys:
        // numpad divide
        // numpad enter

        #endregion
    }
}
