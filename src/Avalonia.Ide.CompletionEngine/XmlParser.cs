﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Avalonia.Ide.CompletionEngine
{
    public class XmlParser
    {
        private readonly ReadOnlyMemory<char> _data;
        private int _parserPos;

        public enum ParserState
        {
            None,
            InsideComment,
            InsideCdata,
            StartElement,
            InsideElement,
            StartAttribute,
            BeforeAttributeValue,
            AttributeValue,
            AfterAttributeValue,
        }

        public ParserState State { get; set; }

        private int _elementNameStart;
        private int _attributeNameStart;
        private int? _elementNameEnd;
        private int? _attributeNameEnd;
        private int _attributeValueStart;
        private Stack<int> _containingTagStart;
        private bool _isClosingTag;

        public string TagName
        {
            get
            {
                if (State < ParserState.StartElement)
                {
                    return null;
                }
                var span = _data.Span;
                if(_elementNameStart >= span.Length)
                {
                    return "";
                }

                if (_elementNameEnd == null)
                {
                    // Check where is element end
                    _elementNameEnd = _elementNameStart;
                    for (int i = _elementNameStart; i < span.Length; i++)
                    {
                        char c = span[i];
                        bool isClosingTag = i == _elementNameStart && c == '/';
                        if (!isClosingTag)
                        {
                            if (char.IsWhiteSpace(c) || c == '/' || c == '>')
                            {
                                _elementNameEnd = i;
                                break;
                            }
                        }

                        _elementNameEnd = i+1;
                    }
                }
                return span.Slice(_elementNameStart, _elementNameEnd.Value - _elementNameStart).ToString();
            }
        }

        public string AttributeName => State >= ParserState.StartAttribute
            ? _data.Span.Slice(_attributeNameStart, (_attributeNameEnd ?? (_data.Length - 1)) - _attributeNameStart + 1).ToString()
            : null;

        public string AttributeValue =>
            State == ParserState.AttributeValue ? _data.Span.Slice(_attributeValueStart).ToString() : null;

        public int? CurrentValueStart =>
            State == ParserState.StartElement
                ? _elementNameStart
                : State == ParserState.StartAttribute
                    ? _attributeNameStart
                    : State == ParserState.AttributeValue
                        ? _attributeValueStart
                        : (int?) null;

        public int? ElementNameEnd => State >= ParserState.StartElement ? _elementNameEnd : null;

        public int ContainingTagStart => _containingTagStart.Count > 0 ? _containingTagStart.Peek() : 0;

        public int NestingLevel => _containingTagStart.Count;

        public int ParserPos => _parserPos;

        public bool IsInClosingTag => _isClosingTag;

        public XmlParser(ReadOnlyMemory<char> data, int start = 0)
        {
            _containingTagStart = new Stack<int>();
            _data = data;
            _parserPos = start;
        }

        private const string CommentStart = "!--";
        private const string CommentEnd = "-->";

        private const string CdataStart = "![CDATA[";
        private const string CdataEnd = "]]>";
        
        bool CheckPrev(int caret, string checkFor)
        {
            var startAt = caret - checkFor.Length + 1;
            if (startAt < 0)
                return false;
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var c = 0; c < checkFor.Length; c++)
            {
                if (_data.Span[c + startAt] != checkFor[c])
                    return false;
            }
            return true;
        }

        private bool ParseChar()
        {
            if(_parserPos >= _data.Length)
            {
                return false;
            }

            int i = _parserPos++;
            var span = _data.Span;
            var c = span[i];
            if (c == '<' && State == ParserState.None)
            {
                State = ParserState.StartElement;
                _isClosingTag = _data.Span.Length > i + 1 && span[i + 1] == '/';
                _elementNameStart = i + 1;
                _elementNameEnd = null;

                _containingTagStart.Push(i);
            }
            else if (State == ParserState.StartElement && CheckPrev(i, CommentStart))
            {
                State = ParserState.InsideComment;
            }
            else if (State == ParserState.InsideComment && CheckPrev(i, CommentEnd))
            {
                State = ParserState.None;
            }
            else if (State == ParserState.StartElement && CheckPrev(i, CdataStart))
            {
                State = ParserState.InsideCdata;
            }
            else if (State == ParserState.InsideCdata && CheckPrev(i, CdataEnd))
            {
                State = ParserState.None;
            }
            else if (State == ParserState.StartElement && char.IsWhiteSpace(c))
            {
                State = ParserState.InsideElement;
                _attributeNameStart = i;
                _elementNameEnd = i - 1;
            }
            else if ((State == ParserState.InsideElement
               || State == ParserState.StartElement
               || State == ParserState.AfterAttributeValue)
                   && c == '/' && CheckPrev(i - 1, "<"))
            {
                if (_containingTagStart.Count > 0)
                {
                    _containingTagStart.Pop();
                }
                if (_containingTagStart.Count > 0)
                {
                    _containingTagStart.Pop();
                }
            }
            else if ((State == ParserState.InsideElement
                || State == ParserState.StartElement
                || State == ParserState.AfterAttributeValue)
                    && c == '>' && CheckPrev(i - 1, "/"))
            {
                State = ParserState.None;
                if (_containingTagStart.Count > 0)
                {
                    _containingTagStart.Pop();
                }
            }
            else if ((State == ParserState.InsideElement
                || State == ParserState.StartElement
                || State == ParserState.AfterAttributeValue)
                    && c == '>')
            {
                State = ParserState.None;
            }
            else if (State == ParserState.InsideElement && (char.IsLetter(c) || c == '_' || c == ':'))
            {
                State = ParserState.StartAttribute;
                _attributeNameStart = i;
                _attributeNameEnd = null;
            }
            else if (State == ParserState.StartAttribute && (c == '=' || char.IsWhiteSpace(c)))
            {
                State = ParserState.BeforeAttributeValue;
                _attributeNameEnd = i - 1;
            }
            else if (State == ParserState.BeforeAttributeValue && c == '"')
            {
                State = ParserState.AttributeValue;
                _attributeValueStart = i + 1;
            }
            else if (State == ParserState.AttributeValue && c == '"')
            {
                State = ParserState.AfterAttributeValue;
            }
            else if (State == ParserState.AfterAttributeValue)
            {
                State = ParserState.InsideElement;
            }
            return true;
        }

        public string GetParentTagName(int level)
        {
            if (NestingLevel - level - 1 < 0)
                return null;
            var start = _containingTagStart.Skip(level).FirstOrDefault();
            var m = Regex.Match(_data.Span.Slice(start).ToString(), @"^<[^\s/>]+");
            if (m.Success)
                return m.Value.Substring(1);
            return null;

        }

        public static XmlParser Parse(string data)
        {
            return Parse(data.AsMemory());
        }

        public static XmlParser Parse(ReadOnlyMemory<char> data)
        {
            return Parse(data, 0, data.Length);
        }

        public static XmlParser Parse(ReadOnlyMemory<char> data, int start, int end)
        {
            var rv = new XmlParser(data, start);
            for (var i = start; i < end; i++)
            {
                if (!rv.ParseChar())
                {
                    break;
                }
            }

            return rv;
        }

        /// <summary>
        /// Try parsing until closing tag is found
        /// </summary>
        public bool SeekClosingTag()
        {
            while (State == ParserState.StartElement)
            {
                if (!ParseChar())
                {
                    return false;
                }
                // leave initial start element
            }

            while (NestingLevel != 0)
            {
                if (!ParseChar())
                {
                    return false;
                }
                // find next element at same level in document
            }

            while (State != ParserState.StartElement)
            {
                if (!ParseChar())
                {
                    return false;
                }
                // find start of next element
            }

            return true;
        }

        public override string ToString()
        {
            return $"State: {State}, TagName: {TagName}, AttributeName: {AttributeName}, Attribute: {AttributeValue}, ContainingTagStart: {ContainingTagStart}";
        }

        /// <summary>
        /// Clone this parser state
        /// </summary>
        /// <returns></returns>
        public XmlParser Clone()
        {
            return (XmlParser)MemberwiseClone();
        }
    }
}
