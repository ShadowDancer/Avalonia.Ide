﻿using System;
using System.Collections.Generic;

namespace Avalonia.Ide.CompletionEngine
{
    public class CloseXmlTagManipulation
    {
        private readonly XmlParser _state;
        private readonly ReadOnlyMemory<char> _text;
        private readonly int _position;

        public CloseXmlTagManipulation(XmlParser state, ReadOnlyMemory<char> text, int position)
        {
            _state = state;
            _text = text;
            _position = position;
        }

        public void TryCloseTag(ITextChange textChange, IList<TextManipulation> manipulations)
        {
            var currentTag = _state.ParseCurrentTagName();
            if (textChange.NewText == "/" && !string.IsNullOrEmpty(currentTag) && currentTag != "/")
            {
                var text = _text.Span;
                int pos = _state.ParserPos;
                char c = ' ';
                while (char.IsWhiteSpace(c) && text.Length > pos + 1)
                {
                    pos++;
                    c = text[pos];
                }

                bool tagAlreadyClosed = c == '>';
                if (!tagAlreadyClosed)
                {
                    manipulations.Add(TextManipulation.Insert(_position + 1, $">"));
                }
                else
                {
                    var closingTagPos = FindClosingTag(currentTag, pos + 1);
                    if(closingTagPos != null)
                    {
                        manipulations.Add(TextManipulation.Insert(_position + 1, $">"));
                        manipulations.Add(TextManipulation.Delete(_position + 1, closingTagPos.Value - _position));
                    }
                }
            }
        }

        /// <summary>
        /// If we just changed open tag to self-closing this finds closing tag if closed tag was empty
        /// </summary>
        /// <returns>Position of closing brace of closing tag if this tag is empty and self closing</returns>
        private int? FindClosingTag(string currentTag, int startPos)
        {
            int pos = startPos;

            var text = _text.Span;
            if (pos >= text.Length)
            {
                return null;
            }

            // skip eventual whitespaces

            char c = text[pos];
            SkipWhitespace(ref pos, text, ref c);

            // Check next text is </closingTag>
            if (text.Length < pos + currentTag.Length + 3
                || text[pos] != '<'
                || text[pos + 1] != '/')
            {
                return null;
            }

            pos = pos + 2;
            for (int i = 0; i < currentTag.Length; i++)
            {
                if (text[pos] != currentTag[i])
                {
                    return null;
                }
                pos++;
            }

            // parse whitespace
            SkipWhitespace(ref pos, text, ref c);

            if(text.Length <= pos || text[pos] != '>')
            {
                return null;
            }

            return pos;
        }

        private static void SkipWhitespace(ref int pos, ReadOnlySpan<char> text, ref char c)
        {
            for (; char.IsWhiteSpace(c) && text.Length > pos + 1; pos++)
            {
                c = text[pos];
            }
        }
    }
}