using System;
using System.Collections.Generic;
using System.Text;
using KeyStringParser;

namespace CSTokenizer
{
    public class CommonHandlerData
    {
        public StringBuilder _buffer;
        public StringBuilder _tokenBuffer;
        public List<Token> _tokens;
        public Action<Type> _changeState;
        public CharacterType PrevCharacterType;
        public CharacterType CurrentCharacterType;
    }

    public abstract class NewHandler
    {
        private readonly CommonHandlerData _commonData;

        private Dictionary<string, Type> _stateChangeStrings;
        private bool _initialized;
        private SequencedDictionary<char, CharacterType> _sequencedDictionary;
        private Dictionary<CharacterType, CharDescriptor> _characterMap;
        private Dictionary<CharacterType, Action> _actionMap;
        private Type _defaultStateChangeType;

        protected NewHandler(CommonHandlerData commonData)
        {
            _commonData = commonData;
        }

        public abstract Dictionary<CharacterType, CharDescriptor> GetCharacterMap();
        protected abstract Dictionary<string, Type> GetStateChangeStrings();
        public abstract Dictionary<CharacterType, Action> GetActionMap();
        public abstract CharacterType GetDefaultCharacterType();
        public virtual Type GetDefaultStateChange()
        {
            return null;
        }
        public virtual Action GetDefaultAction()
        {
            return () => { };
        }

        protected void PushTokenAndReset()
        {
            PushToken();
            _commonData._buffer.Length = 0;
        }

        protected void ProcessControl()
        {
            PushToken();
            AddBufferToToken();
            _commonData.PrevCharacterType = _commonData.CurrentCharacterType;
            PushToken();
        }

        protected void PushToken()
        {
            if (_commonData._tokenBuffer.Length <= 0) return;

            _commonData._tokens.Add(new Token(_commonData.PrevCharacterType, _commonData._tokenBuffer.ToString()));
            _commonData._tokenBuffer.Length = 0;
        }

        protected void AddBufferToToken()
        {
            _commonData._tokenBuffer.Append(_commonData._buffer);
            _commonData._buffer.Length = 0;
        }

        protected void ChangeState()
        {
            _commonData.PrevCharacterType = _commonData.CurrentCharacterType;
            PushToken();

            if (!_stateChangeStrings.ContainsKey(_commonData._buffer.ToString()))
            {
                if (_defaultStateChangeType == null)
                    throw new Exception("Key not found: " + _commonData._buffer);
                _commonData._changeState(_defaultStateChangeType);
            }
            else
                _commonData._changeState(_stateChangeStrings[_commonData._buffer.ToString()]);
            
            _commonData._buffer.Length = 0;
        }

        private void Initialize()
        {
            if (_initialized) return;

            _stateChangeStrings = GetStateChangeStrings();
            _actionMap = GetActionMap();
            _characterMap = GetCharacterMap();
            _defaultStateChangeType = GetDefaultStateChange();

            _sequencedDictionary = new SequencedDictionary<char, CharacterType>();
            _characterMap.Do(pair => pair.Value.GetStrings().Do(s => _sequencedDictionary.Add(s, pair.Key)));

            _initialized = true;
        }

        public void Process(char c)
        {
            Initialize();

            _sequencedDictionary.Push(c);
            while (_sequencedDictionary.Count() > 0)
            {
                var info = _sequencedDictionary.Pop();
                foreach (var key in info.Keys) _commonData._buffer.Append(key);
                _commonData.CurrentCharacterType = info.Value ?? GetDefaultCharacterType();
                if (info.Value == null) GetDefaultAction()();
                else _actionMap[info.Value]();
                _commonData.PrevCharacterType = info.Value;
            }
        }
    }
}