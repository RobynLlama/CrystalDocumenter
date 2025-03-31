/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

namespace CrystalDocumenter.Utilities;

public static partial class Utils
{
  public const string DefaultHeader = """
  <html>
  <head>
    <link rel="stylesheet" href="https://raw.githack.com/hyrious/github-markdown-css/main/dist/dark.css">
    <style type="text/css">
      html, body {
      margin: 0 !important;
      padding: 0 !important;
      }
      .markdown-body {
      margin: 0 !important;
      padding: 1em;
      }
      .markdown-body pre > code {
      white-space: pre-wrap;
      word-break: break-word;
      }
      .markdown-body li p
      {
        margin-top: 0;
        margin-bottom: 4px;
      }
    </style>
  </head>
  <body>
  <div class="markdown-body">
  

  
  """;

  public const string DefaultFooter = """
  </div>
  </body>
  </html>
  """;
}
