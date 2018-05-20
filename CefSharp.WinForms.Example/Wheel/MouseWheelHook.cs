using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CefSharp.WinForms.Example.Wheel{
    class MouseWheelHook{
        protected bool IsCursorOverBrowser => browser.Bounds.Contains(control.PointToClient(Cursor.Position));

        private readonly Control control;
        private readonly ChromiumWebBrowser browser;

        private readonly NativeMethods.HookProc mouseHookDelegate;
        private IntPtr mouseHook;

        public MouseWheelHook(Control control, ChromiumWebBrowser browser){
            this.control = control;
            this.browser = browser;
            this.mouseHookDelegate = MouseHookProc;

            StartMouseHook();
            browser.Disposed += (sender, args) => StopMouseHook();
        }
        
        private void StartMouseHook(){
            if (mouseHook == IntPtr.Zero){
                mouseHook = NativeMethods.SetWindowsHookEx(NativeMethods.WM_MOUSE_LL, mouseHookDelegate, IntPtr.Zero, 0);
            }
        }

        private void StopMouseHook(){
            if (mouseHook != IntPtr.Zero){
                NativeMethods.UnhookWindowsHookEx(mouseHook);
                mouseHook = IntPtr.Zero;
            }
        }

        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam){
            if (nCode == 0){
                int eventType = wParam.ToInt32();

                if (eventType == NativeMethods.WM_MOUSEWHEEL && IsCursorOverBrowser){
                    int delta = NativeMethods.GetMouseHookData(lParam);
                    browser.SendMouseWheelEvent(0, 0, 0, delta, CefEventFlags.None);
                    Debug.WriteLine("wheel delta: "+delta);
                    return NativeMethods.HOOK_HANDLED;
                }
            }

            return NativeMethods.CallNextHookEx(mouseHook, nCode, wParam, lParam);
        }
        
        public const string HTML = @"
<!DOCTYPE html>
<html lang=""en-US"" class=""scroll-v os-windows dark txt-size--12"" id=""tduck"" data-td-font=""smallest"" data-td-theme=""dark"">

<head>
<meta charset=""utf-8"">
<link rel=""stylesheet"" href=""https://ton.twimg.com/tweetdeck-web/web/dist/bundle.1b53e0e287.css"">
<style type=""text/css"">
body { background: rgb(34, 36, 38) !important }
</style>
<style type=""text/css"">
html, body{ height:auto!important; overflow-x:hidden!important; overflow-y:auto!important; }
body::before{ content:none!important; }
.column{ background:transparent!important; }
.scroll-styled-v::-webkit-scrollbar{ width:7px!important; }
.scroll-styled-v::-webkit-scrollbar-thumb{ border-radius:0!important; }
.scroll-styled-v::-webkit-scrollbar-track{ border-left:0!important; }
</style>
</head>

<body class=""scroll-styled-v td-notification"" td-example-notification=""""><div class=""column"" style=""width:100%!important;min-height:100vh!important;height:auto!important;overflow:initial!important;""><article>
<div class=""js-stream-item-content item-box js-show-detail"">
<div class=""js-tweet tweet"">
<header class=""tweet-header"">
<time class=""tweet-timestamp js-timestamp pull-right txt-mute"">
<a target=""_blank"" rel=""url"" href=""https://twitter.com/TryMyAwesomeApp"" class=""txt-size-variable--12"">now</a>
</time>
<a target=""_blank"" rel=""user"" href=""https://twitter.com/TryMyAwesomeApp"" class=""account-link link-complex block"">
<div class=""obj-left item-img tweet-img"">
<img width=""48"" height=""48"" alt=""TryMyAwesomeApp's avatar"" src=""https://pbs.twimg.com/profile_images/765161905312980992/AhDP9iY-_normal.jpg"" class=""tweet-avatar avatar pull-right"">
</div>
<div class=""nbfc"">
<span class=""account-inline txt-ellipsis"">
<b class=""fullname link-complex-target"">TweetDuck</b>
<span class=""username txt-mute"">@TryMyAwesomeApp</span>
</span>
</div>
</a>
</header>
<div class=""tweet-body"">
<p class=""js-tweet-text tweet-text with-linebreaks"">Here you can see the position and appearance of desktop notifications.<br><br>For location and size, you can pick a preset, or select <strong>Custom</strong> and then freely move or resize the window.</p><div style=""margin-top:200px"">Scrollbar test padding...<div style=""margin-top:200px"">Scrollbar test padding...<div style=""margin-top:200px"">Scrollbar test padding...</div>
</div>
</div>
</div>
</article>
</div></body></html>";
    }
}
