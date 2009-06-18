using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSTokenizer
{
    public class Handler
    {
        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly StringBuilder _tokenBuffer = new StringBuilder();
        private readonly List<Token> _tokens = new List<Token>();
        private NewHandler _handler;
        private CharacterType _currentCharacterType;
        private CharacterType _prevCharacterType;
        private readonly Dictionary<Type, NewHandler> _handlerLookup = new Dictionary<Type, NewHandler>();

        public Handler()
        {
            ChangeState(typeof (CodeHandler));
        }

        private void ChangeState(Type handlerType)
        {
            if (!_handlerLookup.ContainsKey(handlerType))
            {
                var commonData = new CommonHandlerData
                                     {
                                         _buffer = _buffer,
                                         _tokenBuffer = _tokenBuffer,
                                         _tokens = _tokens,
                                         _changeState = ChangeState,
                                         CurrentCharacterType = _currentCharacterType,
                                         PrevCharacterType = _prevCharacterType
                                     };
                try
                {
                    var obj = Activator.CreateInstance(handlerType, commonData);
                    _handlerLookup[obj.GetType()] = (NewHandler)obj;
                }
                catch (Exception)
                {
                    throw new Exception("Failed changing state with buffer: " + _buffer);
                }
            }
            _handler = _handlerLookup[handlerType];
        }
        
        public void Process(char c)
        {
            _handler.Process(c);
        }

        public int Count()
        {
            return _tokens.Count();
        }

        public Token Pop()
        {
            var token = _tokens.First();
            _tokens.RemoveAt(0);
            return token;
        }

        public void Flush()
        {
            if (_tokenBuffer.Length == 0) return;

            _tokens.Add(new Token(_currentCharacterType, _tokenBuffer.ToString()));
            _tokenBuffer.Length = 0;
        }
    }
}