// Copyright (c) 2013-2018 Cemalettin Dervis, MIT License.
// https://github.com/cemdervis/SharpConfig

using System;
using System.IO;
using System.Text;

namespace SharpConfig
{
  internal static class ConfigurationReader
  {
    internal static Configuration ReadFromString(string source)
    {
      var config = new Configuration();

      using (var reader = new StringReader(source))
      {
        Parse(reader, config);
      }

      return config;
    }

    private static void Parse(StringReader reader, Configuration config)
    {
      Section currentSection = new Section(Section.DefaultSectionName);
      var preCommentBuilder = new StringBuilder();

      int newlineLength = Environment.NewLine.Length;
      int lineNumber = 0;

      string line;

      // Read until EOF.
      while ((line = reader.ReadLine()) != null)
      {
        lineNumber++;

        // Remove all leading/trailing white-spaces.
        line = line.Trim();

        // Skip empty lines.
        if (string.IsNullOrEmpty(line))
          continue;

        var comment = ParseComment(line, out int commentIndex);

        if (commentIndex == 0)
        {
          // pre-comment
          if (!Configuration.IgnorePreComments)
          {
            preCommentBuilder.AppendLine(comment);
          }

          continue;
        }

        string lineWithoutComment = line;
        if (commentIndex > 0)
        {
          // inline comment
          lineWithoutComment = line.Remove(commentIndex).Trim();
        }

        if (lineWithoutComment.StartsWith("[")) // Section
        {
          if (currentSection != null && currentSection.Name == Section.DefaultSectionName && currentSection.SettingCount > 0)
            config.mSections.Add(currentSection);

          currentSection = ParseSection(lineWithoutComment, lineNumber);

          if (!Configuration.IgnoreInlineComments)
            currentSection.Comment = comment;

          if (!Configuration.IgnorePreComments && preCommentBuilder.Length > 0)
          {
            // Remove the last line.
            preCommentBuilder.Remove(preCommentBuilder.Length - newlineLength, newlineLength);
            currentSection.PreComment = preCommentBuilder.ToString();
            preCommentBuilder.Length = 0; // Clear the SB
          }

          config.mSections.Add(currentSection);
        }
        else // Setting
        {
          var setting = ParseSetting(Configuration.IgnoreInlineComments ? line : lineWithoutComment, lineNumber);

          if (!Configuration.IgnoreInlineComments)
            setting.Comment = comment;

          if (currentSection == null)
          {
            throw new ParserException(string.Format(
                "The setting '{0}' has to be in a section.",
                setting.Name), lineNumber);
          }

          if (!Configuration.IgnorePreComments && preCommentBuilder.Length > 0)
          {
            // Remove the last line.
            preCommentBuilder.Remove(preCommentBuilder.Length - newlineLength, newlineLength);
            setting.PreComment = preCommentBuilder.ToString();
            preCommentBuilder.Length = 0; // Clear the SB
          }

          currentSection.Add(setting);
        }
      }
    }

    private static string ParseComment(string line, out int commentCharIndex)
    {
      // A comment starts with a valid comment character that:
      // 1. is not within a quote (eg. "this is # not a comment"), and
      // 2. is not escaped (eg. this is \# not a comment either).
      //
      // A quote has two quotation marks, neither of which is escaped.
      // For example: "this is a quote \" with an escaped quotation mark inside of it"

      string comment = null;
      commentCharIndex = -1;

      var index = 0;
      var quoteCount = 0;
      while (line.Length > index) // traverse line from left to right
      {
        var isValidCommentChar = Array.IndexOf(Configuration.ValidCommentChars, line[index]) > -1;
        var isQuotationMark = line[index] == '\"';
        var isCharWithinQuotes = quoteCount % 2 == 1;
        var isCharEscaped = index > 0 && line[index - 1] == '\\';

        if (isValidCommentChar && !isCharWithinQuotes && !isCharEscaped)
          break; // a comment has started

        if (isQuotationMark && !isCharEscaped)
          quoteCount++; // a non-escaped quotation mark has been found

        index++;
      }

      if (index < line.Length)
      {
        // The end of the string has not been reached => index points to a valid comment character.
        commentCharIndex = index;

        // If it's not the last character, extract the comment.
        if (line.Length > index + 1)
          comment = line.Substring(index + 1).TrimStart();
      }

      return comment;
    }

    private static Section ParseSection(string line, int lineNumber)
    {
      // Format(s) of a section:
      // 1) [<name>]
      //      name may contain any char, including '[' and ']'

      int closingBracketIndex = line.LastIndexOf(']');
      if (closingBracketIndex < 0)
        throw new ParserException("closing bracket missing.", lineNumber);

      // See if there are unwanted chars after the closing bracket.
      if ((line.Length - 1) > closingBracketIndex)
      {
        // Get the part after the raw value to determien whether it's just an inline comment.
        // If so, it's not an unwanted part; otherwise we should notify that it's something unexpected.
        var endPart = line.Substring(closingBracketIndex + 1).Trim();
        if (endPart.IndexOfAny(Configuration.ValidCommentChars) != 0)
        {
          string unwantedToken = line.Substring(closingBracketIndex + 1);

          throw new ParserException(string.Format(
              "unexpected token '{0}'", unwantedToken),
              lineNumber);
        }
      }

      // Extract the section name, and trim all leading / trailing white-spaces.
      string sectionName = line.Substring(1, line.Length - 2).Trim();

      // Otherwise, return a fresh section.
      return new Section(sectionName);
    }

    private static Setting ParseSetting(string line, int lineNumber)
    {
      // Format(s) of a setting:
      // 1) <name> = <value>
      //      name may not contain a '='
      // 2) "<name>" = <value>
      //      name may contain any char, including '='

      string settingName = null;

      // Parse the name first.
      bool isQuotedName = line.StartsWith("\"");

      int equalSignIndex;
      if (isQuotedName)
      {
        // Format 2
        int index = 0;
        do
        {
          index = line.IndexOf('\"', index + 1);
        }
        while (index > 0 && line[index - 1] == '\\');

        if (index < 0)
        {
          throw new ParserException("closing quote mark expected.", lineNumber);
        }

        // Don't trim the name. Quoted names should be taken verbatim.
        settingName = line.Substring(1, index - 1);

        equalSignIndex = line.IndexOf('=', index + 1);
      }
      else
      {
        // Format 1
        equalSignIndex = line.IndexOf('=');
      }

      // Find the assignment operator.
      if (equalSignIndex < 0)
        throw new ParserException("setting assignment expected.", lineNumber);

      if (!isQuotedName)
      {
        settingName = line.Substring(0, equalSignIndex).Trim();
      }

      // Trim the setting name and value.
      string settingValue = line.Substring(equalSignIndex + 1);
      settingValue = settingValue.Trim();

      // Check if non-null name / value is given.
      if (string.IsNullOrEmpty(settingName))
        throw new ParserException("setting name expected.", lineNumber);

      return new Setting(settingName, settingValue);
    }

    internal static Configuration ReadFromBinaryStream(Stream stream, BinaryReader reader)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      if (reader == null)
        reader = new BinaryReader(stream);

      var config = new Configuration();

      int sectionCount = reader.ReadInt32();

      for (int i = 0; i < sectionCount; ++i)
      {
        string sectionName = reader.ReadString();
        int settingCount = reader.ReadInt32();

        var section = new Section(sectionName);

        ReadCommentsBinary(reader, section);

        for (int j = 0; j < settingCount; j++)
        {
          var setting = new Setting(reader.ReadString())
          {
            RawValue = reader.ReadString()
          };

          ReadCommentsBinary(reader, setting);

          section.Add(setting);
        }

        config.Add(section);
      }

      return config;
    }

    private static void ReadCommentsBinary(BinaryReader reader, ConfigurationElement element)
    {
      bool hasComment = reader.ReadBoolean();
      if (hasComment)
      {
        // Read the comment char, but don't do anything with it.
        // This is just for backwards-compatibility.
        reader.ReadChar();
        element.Comment = reader.ReadString();
      }

      bool hasPreComment = reader.ReadBoolean();
      if (hasPreComment)
      {
        // Same as above.
        reader.ReadChar();
        element.PreComment = reader.ReadString();
      }
    }
  }
}
