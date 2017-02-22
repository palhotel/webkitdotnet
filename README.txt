WebKit.NET README
*****************

WebKit.NET is a control library wrapper for WebKit written in C#.

Currently the source code is licensed under a BSD open source license, see
LICENSE.txt for details.

This package currently contains 3 VS solutions.
WebKit.NET.sln		- VS2008 .Net 2.0.
WebKit.NET_2010.sln	- VS2010 .Net 2.0.
WebKit.net4.0.sln	- VS2012 .Net 4.5.

WebKit library (including dependencies) are in the following folders:

    WebKitCore - WebKit COM wrapper library.

    WebKitBrowser - WebKit.NET control library.
    
    WebKitBrowserTest - A simple application that uses the WebKit.NET control
                        to display web pages.

    JSCore - JavaScript bridge/interop between WebKit and WebKitBrowser.

    JSCore.Test - JavaScript bridge/interop unit-tests.

    webkit - The WebKit library and dependencies.

The current releases are at a very early stage of development and are not
recommended for production level use yet.

Please send any questions or feedback to webkitdotnet@peterdn.com.
