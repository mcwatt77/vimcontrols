﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VIMControls.Contracts;
using VIMControls.Controls.StackProcessor;
using VIMControls.Controls.StackProcessor.MathExpressions;

namespace VIMControls
{
    public class KeyMapper
    {
        private static readonly Dictionary<CommandMode, Dictionary<KeyStates, List<IDictionary>>> _commands =
            new Dictionary<CommandMode, Dictionary<KeyStates, List<IDictionary>>>
            {
                {CommandMode.Navigation, new Dictionary<KeyStates, List<IDictionary>>
                                         {
                                             {
                                                 KeyStates.None, new List<IDictionary>
                                                                     {
                                                                         new Dictionary<Key, Action<IVIMPositionController>>
                                                                             {
                                                                                 {Key.NumPad0, c => c.Move(new GridLength(0, GridUnitType.Star), default(GridLength) )},
                                                                                 {Key.NumPad1, c => c.Move(new GridLength(0.1, GridUnitType.Star), default(GridLength) )},
                                                                                 {Key.NumPad2, c => c.Move(new GridLength(0.2, GridUnitType.Star), default(GridLength) )},
                                                                                 {Key.NumPad3, c => c.Move(new GridLength(0.3, GridUnitType.Star), default(GridLength) )},
                                                                                 {Key.NumPad4, c => c.Move(new GridLength(0.4, GridUnitType.Star), default(GridLength) )},
                                                                                 {Key.NumPad5, c => c.Move(new GridLength(0.5, GridUnitType.Star), default(GridLength) )},
                                                                                 {Key.NumPad6, c => c.Move(new GridLength(0.6, GridUnitType.Star), default(GridLength) )},
                                                                                 {Key.NumPad7, c => c.Move(new GridLength(0.7, GridUnitType.Star), default(GridLength) )},
                                                                                 {Key.NumPad8, c => c.Move(new GridLength(0.8, GridUnitType.Star), default(GridLength) )},
                                                                                 {Key.NumPad9, c => c.Move(new GridLength(0.9, GridUnitType.Star), default(GridLength) )},
                                                                                 {Key.P, c => c.TogglePositionIndicator()}
                                                                             },
                                                                         new Dictionary<Key, Action<IVIMMotionController>>
                                                                             {
                                                                                 {Key.J, c => c.MoveVertically(1)},
                                                                                 {Key.K, c => c.MoveVertically(-1)},
                                                                                 {Key.H, c => c.MoveHorizontally(-1)},
                                                                                 {Key.L, c => c.MoveHorizontally(1)}
                                                                             },
                                                                         new Dictionary<Key, Action<IVIMController>>
                                                                             {
                                                                                 {Key.Escape, c => c.ResetInput()}
                                                                             },
                                                                         new Dictionary<Key, Action<IVIMSystemUICommands>>
                                                                             {
                                                                                 {Key.F11, c => c.Maximize()}
                                                                             }
                                                                     }
                                             }
                                         }
                },
                {
                    CommandMode.Command, new Dictionary<KeyStates, List<IDictionary>>
                                         {
                                             {
                                                 KeyStates.None, new List<IDictionary>
                                                                 {
                                                                     new Dictionary<Key, Action<IVIMCommandController>>
                                                                     {
                                                                         {Key.A, c => c.CommandCharacter('a')},
                                                                         {Key.B, c => c.CommandCharacter('b')},
                                                                         {Key.C, c => c.CommandCharacter('c')},
                                                                         {Key.D, c => c.CommandCharacter('d')},
                                                                         {Key.E, c => c.CommandCharacter('e')},
                                                                         {Key.F, c => c.CommandCharacter('f')},
                                                                         {Key.G, c => c.CommandCharacter('g')},
                                                                         {Key.H, c => c.CommandCharacter('h')},
                                                                         {Key.I, c => c.CommandCharacter('i')},
                                                                         {Key.J, c => c.CommandCharacter('j')},
                                                                         {Key.K, c => c.CommandCharacter('k')},
                                                                         {Key.L, c => c.CommandCharacter('l')},
                                                                         {Key.M, c => c.CommandCharacter('m')},
                                                                         {Key.N, c => c.CommandCharacter('n')},
                                                                         {Key.O, c => c.CommandCharacter('o')},
                                                                         {Key.P, c => c.CommandCharacter('p')},
                                                                         {Key.Q, c => c.CommandCharacter('q')},
                                                                         {Key.R, c => c.CommandCharacter('r')},
                                                                         {Key.S, c => c.CommandCharacter('s')},
                                                                         {Key.T, c => c.CommandCharacter('t')},
                                                                         {Key.U, c => c.CommandCharacter('u')},
                                                                         {Key.V, c => c.CommandCharacter('v')},
                                                                         {Key.W, c => c.CommandCharacter('w')},
                                                                         {Key.X, c => c.CommandCharacter('x')},
                                                                         {Key.Y, c => c.CommandCharacter('y')},
                                                                         {Key.Z, c => c.CommandCharacter('z')},
                                                                         {Key.Space, c => c.CommandCharacter(' ')},
                                                                         {Key.OemPeriod, c => c.CommandCharacter('.')},
                                                                         {Key.Enter, c => c.Execute()},
                                                                         {Key.NumPad0, c => c.CommandCharacter('0')},
                                                                         {Key.NumPad1, c => c.CommandCharacter('1')},
                                                                         {Key.NumPad2, c => c.CommandCharacter('2')},
                                                                         {Key.NumPad3, c => c.CommandCharacter('3')},
                                                                         {Key.NumPad4, c => c.CommandCharacter('4')},
                                                                         {Key.NumPad5, c => c.CommandCharacter('5')},
                                                                         {Key.NumPad6, c => c.CommandCharacter('6')},
                                                                         {Key.NumPad7, c => c.CommandCharacter('7')},
                                                                         {Key.NumPad8, c => c.CommandCharacter('8')},
                                                                         {Key.NumPad9, c => c.CommandCharacter('9')},
                                                                         {Key.Add, c => c.CommandCharacter('+')},
                                                                         {Key.Subtract, c => c.CommandCharacter('-')},
                                                                         {Key.Multiply, c => c.CommandCharacter('*')},
                                                                         {Key.Divide, c => c.CommandCharacter('/')},
                                                                         {Key.Back, c => c.CommandBackspace()}
                                                                     },
                                                                     new Dictionary<Key, Action<IVIMControlContainer>>
                                                                     {
                                                                         {Key.Escape, c => c.ResetInput()}
                                                                     }
                                                                 }
                                                 }

                                         }
                    },

                {
                    CommandMode.Insert, new Dictionary<KeyStates, List<IDictionary>>
                                        {
                                            {
                                                KeyStates.None, new List<IDictionary>
                                                                {

                                                                    new Dictionary<Key, Action<IVIMCharacterController>>
                                                                    {
                                                                        {Key.A, c => c.Output('a')},
                                                                        {Key.B, c => c.Output('b')},
                                                                        {Key.C, c => c.Output('c')},
                                                                        {Key.D, c => c.Output('d')},
                                                                        {Key.E, c => c.Output('e')},
                                                                        {Key.F, c => c.Output('f')},
                                                                        {Key.G, c => c.Output('g')},
                                                                        {Key.H, c => c.Output('h')},
                                                                        {Key.I, c => c.Output('i')},
                                                                        {Key.J, c => c.Output('j')},
                                                                        {Key.K, c => c.Output('k')},
                                                                        {Key.L, c => c.Output('l')},
                                                                        {Key.M, c => c.Output('m')},
                                                                        {Key.N, c => c.Output('n')},
                                                                        {Key.O, c => c.Output('o')},
                                                                        {Key.P, c => c.Output('p')},
                                                                        {Key.Q, c => c.Output('q')},
                                                                        {Key.R, c => c.Output('r')},
                                                                        {Key.S, c => c.Output('s')},
                                                                        {Key.T, c => c.Output('t')},
                                                                        {Key.U, c => c.Output('u')},
                                                                        {Key.V, c => c.Output('v')},
                                                                        {Key.W, c => c.Output('w')},
                                                                        {Key.X, c => c.Output('x')},
                                                                        {Key.Y, c => c.Output('y')},
                                                                        {Key.Z, c => c.Output('z')},
                                                                        {Key.Space, c => c.Output(' ')},
                                                                        {Key.OemPeriod, c => c.Output('.')},
                                                                        {Key.OemComma, c => c.Output(',')},
                                                                        {Key.OemSemicolon, c => c.Output(';')},
                                                                        {Key.OemQuotes, c => c.Output('\'')},
                                                                        {Key.D0, c => c.Output('0')},
                                                                        {Key.D1, c => c.Output('1')},
                                                                        {Key.D2, c => c.Output('2')},
                                                                        {Key.D3, c => c.Output('3')},
                                                                        {Key.D4, c => c.Output('4')},
                                                                        {Key.D5, c => c.Output('5')},
                                                                        {Key.D6, c => c.Output('6')},
                                                                        {Key.D7, c => c.Output('7')},
                                                                        {Key.D8, c => c.Output('8')},
                                                                        {Key.D9, c => c.Output('9')},
                                                                        {Key.NumPad0, c => c.Output('0')},
                                                                        {Key.NumPad1, c => c.Output('1')},
                                                                        {Key.NumPad2, c => c.Output('2')},
                                                                        {Key.NumPad3, c => c.Output('3')},
                                                                        {Key.NumPad4, c => c.Output('4')},
                                                                        {Key.NumPad5, c => c.Output('5')},
                                                                        {Key.NumPad6, c => c.Output('6')},
                                                                        {Key.NumPad7, c => c.Output('7')},
                                                                        {Key.NumPad8, c => c.Output('8')},
                                                                        {Key.NumPad9, c => c.Output('9')},
                                                                        {Key.Add, c => c.Output('+')},
                                                                        {Key.Subtract, c => c.Output('-')},
                                                                        {Key.Multiply, c => c.Output('*')},
                                                                        {Key.Divide, c => c.Output('/')},
                                                                        {Key.Enter, c => c.NewLine()},
                                                                        {Key.Back, c => c.Backspace()},
                                                                        {Key.Tab, c => c.Output('\t')}
                                                                    },
                                                                    new Dictionary<Key, Action<IVIMActionController>>
                                                                    {
                                                                        {Key.Escape, c => c.EnterNormalMode()}
                                                                    },
                                                                    new Dictionary<Key, Action<IVIMSystemUICommands>>
                                                                    {
                                                                        {Key.F12, c => c.Save()}
                                                                    }
                                                                }
                                                },
                                            {
                                                KeyStates.Ctrl, new List<IDictionary>
                                                                {

                                                                    new Dictionary<Key, Action<IVIMActionController>>
                                                                    {
                                                                        {Key.C, c => c.EnterNormalMode()}
                                                                    }
                                                                }
                                                },
                                            {
                                                KeyStates.Shift, new List<IDictionary>
                                                                 {

                                                                     new Dictionary
                                                                         <Key, Action<IVIMCharacterController>>
                                                                     {
                                                                         {Key.A, c => c.Output('A')},
                                                                         {Key.B, c => c.Output('B')},
                                                                         {Key.C, c => c.Output('C')},
                                                                         {Key.D, c => c.Output('D')},
                                                                         {Key.E, c => c.Output('E')},
                                                                         {Key.F, c => c.Output('F')},
                                                                         {Key.G, c => c.Output('G')},
                                                                         {Key.H, c => c.Output('H')},
                                                                         {Key.I, c => c.Output('I')},
                                                                         {Key.J, c => c.Output('J')},
                                                                         {Key.K, c => c.Output('K')},
                                                                         {Key.L, c => c.Output('L')},
                                                                         {Key.M, c => c.Output('M')},
                                                                         {Key.N, c => c.Output('N')},
                                                                         {Key.O, c => c.Output('O')},
                                                                         {Key.P, c => c.Output('P')},
                                                                         {Key.Q, c => c.Output('Q')},
                                                                         {Key.R, c => c.Output('R')},
                                                                         {Key.S, c => c.Output('S')},
                                                                         {Key.T, c => c.Output('T')},
                                                                         {Key.U, c => c.Output('U')},
                                                                         {Key.V, c => c.Output('V')},
                                                                         {Key.W, c => c.Output('W')},
                                                                         {Key.X, c => c.Output('X')},
                                                                         {Key.Y, c => c.Output('Y')},
                                                                         {Key.Z, c => c.Output('Z')},
                                                                         {Key.OemQuestion, c => c.Output('?')},
                                                                         {Key.OemSemicolon, c => c.Output(':')},
                                                                         {Key.OemQuotes, c => c.Output('"')},
                                                                         {Key.D0, c => c.Output(')')},
                                                                         {Key.D1, c => c.Output('!')},
                                                                         {Key.D2, c => c.Output('@')},
                                                                         {Key.D3, c => c.Output('#')},
                                                                         {Key.D4, c => c.Output('$')},
                                                                         {Key.D5, c => c.Output('%')},
                                                                         {Key.D6, c => c.Output('^')},
                                                                         {Key.D7, c => c.Output('&')},
                                                                         {Key.D8, c => c.Output('*')},
                                                                         {Key.D9, c => c.Output('(')},
                                                                         {Key.Space, c => c.Output(' ')}
                                                                     }
                                                                 }
                                                }
                                        }
                    },
                {
                    CommandMode.StackInsert, new Dictionary<KeyStates, List<IDictionary>>
                                        {
                                            {
                                                KeyStates.None, new List<IDictionary>
                                                                {
                                                                    new Dictionary<Key, Action<IStackInputController>>
                                                                    {
                                                                        {Key.A, c => c.Output('a')},
                                                                        {Key.B, c => c.Output('b')},
                                                                        {Key.C, c => c.Output('c')},
                                                                        {Key.D, c => c.Output('d')},
                                                                        {Key.E, c => c.Output('e')},
                                                                        {Key.F, c => c.Output('f')},
                                                                        {Key.G, c => c.Output('g')},
                                                                        {Key.H, c => c.Output('h')},
                                                                        {Key.I, c => c.Output('i')},
                                                                        {Key.J, c => c.Output('j')},
                                                                        {Key.K, c => c.Output('k')},
                                                                        {Key.L, c => c.Output('l')},
                                                                        {Key.M, c => c.Output('m')},
                                                                        {Key.N, c => c.Output('n')},
                                                                        {Key.O, c => c.Output('o')},
                                                                        {Key.P, c => c.Output('p')},
                                                                        {Key.Q, c => c.Output('q')},
                                                                        {Key.R, c => c.Output('r')},
                                                                        {Key.S, c => c.Output('s')},
                                                                        {Key.T, c => c.Output('t')},
                                                                        {Key.U, c => c.Output('u')},
                                                                        {Key.V, c => c.Output('v')},
                                                                        {Key.W, c => c.Output('w')},
                                                                        {Key.X, c => c.Output('x')},
                                                                        {Key.Y, c => c.Output('y')},
                                                                        {Key.Z, c => c.Output('z')},
                                                                        {Key.Space, c => c.Output(' ')},
                                                                        {Key.OemPeriod, c => c.Output('.')},
                                                                        {Key.OemComma, c => c.Output(',')},
                                                                        {Key.OemSemicolon, c => c.Output(';')},
                                                                        {Key.OemQuotes, c => c.Output('\'')},
                                                                        {Key.D0, c => c.Output('0')},
                                                                        {Key.D1, c => c.Output('1')},
                                                                        {Key.D2, c => c.Output('2')},
                                                                        {Key.D3, c => c.Output('3')},
                                                                        {Key.D4, c => c.Output('4')},
                                                                        {Key.D5, c => c.Output('5')},
                                                                        {Key.D6, c => c.Output('6')},
                                                                        {Key.D7, c => c.Output('7')},
                                                                        {Key.D8, c => c.Output('8')},
                                                                        {Key.D9, c => c.Output('9')},
                                                                        {Key.NumPad0, c => c.Output('0')},
                                                                        {Key.NumPad1, c => c.Output('1')},
                                                                        {Key.NumPad2, c => c.Output('2')},
                                                                        {Key.NumPad3, c => c.Output('3')},
                                                                        {Key.NumPad4, c => c.Output('4')},
                                                                        {Key.NumPad5, c => c.Output('5')},
                                                                        {Key.NumPad6, c => c.Output('6')},
                                                                        {Key.NumPad7, c => c.Output('7')},
                                                                        {Key.NumPad8, c => c.Output('8')},
                                                                        {Key.NumPad9, c => c.Output('9')},
                                                                        {Key.Enter, c => c.NewLine()},
                                                                        {Key.Back, c => c.Backspace()},
                                                                        {Key.Tab, c => c.Output('\t')},
                                                                        {Key.OemMinus, c => c.Function(StackOpExpression.Subtract)},
                                                                        {Key.Add, c => c.Function(StackOpExpression.Add)},
                                                                        {Key.Subtract, c => c.Function(StackOpExpression.Subtract)},
                                                                        {Key.Multiply, c => c.Function(StackOpExpression.Multiply)},
                                                                        {Key.Divide, c => c.Function(StackOpExpression.Divide)}
                                                                    },
                                                                    new Dictionary<Key, Action<IVIMActionController>>
                                                                    {
                                                                        {Key.Escape, c => c.EnterNormalMode()}
                                                                    },
                                                                    new Dictionary<Key, Action<IVIMSystemUICommands>>
                                                                    {
                                                                        {Key.F12, c => c.Save()}
                                                                    }
                                                                }
                                                },
                                            {
                                                KeyStates.Ctrl, new List<IDictionary>
                                                                {
                                                                    new Dictionary<Key, Action<IVIMActionController>>
                                                                    {
                                                                        {Key.C, c => c.EnterNormalMode()}
                                                                    }
                                                                }
                                                },
                                            {
                                                KeyStates.Shift, new List<IDictionary>
                                                                 {

                                                                     new Dictionary
                                                                         <Key, Action<IVIMCharacterController>>
                                                                     {
                                                                         {Key.A, c => c.Output('A')},
                                                                         {Key.B, c => c.Output('B')},
                                                                         {Key.C, c => c.Output('C')},
                                                                         {Key.D, c => c.Output('D')},
                                                                         {Key.E, c => c.Output('E')},
                                                                         {Key.F, c => c.Output('F')},
                                                                         {Key.G, c => c.Output('G')},
                                                                         {Key.H, c => c.Output('H')},
                                                                         {Key.I, c => c.Output('I')},
                                                                         {Key.J, c => c.Output('J')},
                                                                         {Key.K, c => c.Output('K')},
                                                                         {Key.L, c => c.Output('L')},
                                                                         {Key.M, c => c.Output('M')},
                                                                         {Key.N, c => c.Output('N')},
                                                                         {Key.O, c => c.Output('O')},
                                                                         {Key.P, c => c.Output('P')},
                                                                         {Key.Q, c => c.Output('Q')},
                                                                         {Key.R, c => c.Output('R')},
                                                                         {Key.S, c => c.Output('S')},
                                                                         {Key.T, c => c.Output('T')},
                                                                         {Key.U, c => c.Output('U')},
                                                                         {Key.V, c => c.Output('V')},
                                                                         {Key.W, c => c.Output('W')},
                                                                         {Key.X, c => c.Output('X')},
                                                                         {Key.Y, c => c.Output('Y')},
                                                                         {Key.Z, c => c.Output('Z')},
                                                                         {Key.OemQuestion, c => c.Output('?')},
                                                                         {Key.OemSemicolon, c => c.Output(':')},
                                                                         {Key.OemQuotes, c => c.Output('"')},
                                                                         {Key.D0, c => c.Output(')')},
                                                                         {Key.D1, c => c.Output('!')},
                                                                         {Key.D2, c => c.Output('@')},
                                                                         {Key.D3, c => c.Output('#')},
                                                                         {Key.D4, c => c.Output('$')},
                                                                         {Key.D5, c => c.Output('%')},
                                                                         {Key.D6, c => c.Output('^')},
                                                                         {Key.D7, c => c.Output('&')},
                                                                         {Key.D8, c => c.Output('*')},
                                                                         {Key.D9, c => c.Output('(')}
                                                                     },
                                                                    new Dictionary<Key, Action<IStackInputController>>
                                                                    {
                                                                        {Key.OemPlus, c => c.Function(StackOpExpression.Add)}
                                                                    }
                                                                 }
                                                }
                                        }
                    },

                {
                    CommandMode.Normal, new Dictionary<KeyStates, List<IDictionary>>
                                        {
                                            {
                                                KeyStates.None, new List<IDictionary>
                                                                {
                                                                    new Dictionary<Key, Action<IVIMActionController>>
                                                                    {
                                                                        {
                                                                            Key.I,
                                                                            c =>
                                                                            c.EnterInsertMode(CharacterInsertMode.Before)
                                                                            },
                                                                        {Key.O, c => c.InsertLine(LineInsertMode.Below)},
                                                                        {Key.X, c => c.DeleteAtCursor()},
                                                                        {
                                                                            Key.A,
                                                                            c => c.EnterInsertMode(CharacterInsertMode.After)
                                                                            }
                                                                    },
                                                                    new Dictionary<Key, Action<IVIMMotionController>>
                                                                    {
                                                                        {Key.H, c => c.MoveHorizontally(-1)},
                                                                        {Key.L, c => c.MoveHorizontally(1)},
                                                                        {Key.J, c => c.MoveVertically(1)},
                                                                        {Key.K, c => c.MoveVertically(-1)},
                                                                        {Key.D0, c => c.BeginningOfLine()},
                                                                        {Key.Enter, c => c.NextLine()}
                                                                    },
                                                                    new Dictionary<Key, Action<IVIMContainer>>
                                                                    {
//                                                                        {Key.F2, c => c.Navigate("graph")},
                                                                        {Key.F3, c => c.Navigate("form")},
                                                                        {Key.F4, c => c.Navigate("mru")}
                                                                    },
                                                                    new Dictionary<Key, Action<IVIMSystemUICommands>>
                                                                    {
                                                                        {Key.F11, c => c.Maximize()},
                                                                        {Key.F12, c => c.Save()}
                                                                    },
                                                                    new Dictionary<Key, Action<IVIMControlContainer>>
                                                                    {
                                                                        {Key.Escape, c => c.ResetInput()}
                                                                    }
                                                                }

                                                },
                                            {
                                                KeyStates.Shift, new List<IDictionary>
                                                                 {

                                                                     new Dictionary<Key, Action<IVIMMotionController>>
                                                                     {
                                                                         {Key.D4, c => c.EndOfLine()}
                                                                     },
                                                                     new Dictionary<Key, Action<IVIMActionController>>
                                                                     {
                                                                         {Key.O, c => c.InsertLine(LineInsertMode.Above)},
                                                                         {Key.OemSemicolon, c => c.EnterCommandMode()}
                                                                     },
                                                                     new Dictionary<Key, Action<IVIMExpressionProcessor>>
                                                                     {
                                                                         {Key.Q, c => { c.Push(new TextPointerExpression());
                                                                             VIMMessageService.SendMessage<IVIMControlContainer>(a => a.Focus(c));}}
                                                                     }
                                                                 }

                                                }
                                        }
                    }
            };

        public static IEnumerable<IVIMAction> GetVIMCommand(CommandMode mode, Key keyStroke, KeyStates keyStates)
        {
            yield return _commands.Get(mode).Get(keyStates).Get(keyStroke);
        }
    }

    public static class Fluent
    {
        public static IVIMAction Get(this IEnumerable<IDictionary> src, Key keyStroke)
        {
            if (src == null)
                return new VIMAction();

            var dict = src.FirstOrDefault(d => d.Contains(keyStroke));
            if (dict != null)
                return new VIMAction(dict[keyStroke]);
            return new VIMAction();
        }
        public static IEnumerable<IDictionary> Get(this IDictionary src, KeyStates keyStates)
        {
            if (src != null ? src.Contains(keyStates) : false)
                return src[keyStates] as IEnumerable<IDictionary>;
            return null;
        }

        public static IDictionary Get(this IDictionary src, CommandMode mode)
        {
            if (src != null ? src.Contains(mode) : false)
                return src[mode] as IDictionary;
            return null;
        }
    }

    [Flags]
    public enum KeyStates
    {
        None = 0, Ctrl = 1, Shift = 2, Alt = 4
    }

    public enum CommandMode
    {
        Normal, Insert, Command, Visual, Navigation, StackInsert
    }

    public enum NumberMapperType
    {
        KeyPad,
        Standard,
        Both
    }

    public interface INumberMapper : IKeyMapper
    {
        Func<int, object[]> Map();
    }

    public interface IKeyMapper
    {
    }

    public abstract class MapperAttribute : Attribute
    {
        public abstract IEnumerable<KeyValuePair<Key, object[]>> GetMappings();
    }

    public class NumberMapperAttribute : MapperAttribute
    {
        private readonly NumberMapperType _pad;
        private readonly Type _mapperType;

        public NumberMapperAttribute(NumberMapperType pad, Type mapperType)
        {
            _pad = pad;
            if (_pad != NumberMapperType.KeyPad) throw new Exception("Currently only numpad keys supported");

            _mapperType = mapperType;
        }

        public override IEnumerable<KeyValuePair<Key, object[]>> GetMappings()
        {
            if (!(_mapperType is INumberMapper)) throw new Exception("Invalid type specified for NumberMapper");
            var mapper = (INumberMapper)_mapperType.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

            yield return new KeyValuePair<Key, object[]>(Key.NumPad0, mapper.Map()(0));
            yield return new KeyValuePair<Key, object[]>(Key.NumPad1, mapper.Map()(1));
            yield return new KeyValuePair<Key, object[]>(Key.NumPad2, mapper.Map()(2));
            yield return new KeyValuePair<Key, object[]>(Key.NumPad3, mapper.Map()(3));
            yield return new KeyValuePair<Key, object[]>(Key.NumPad4, mapper.Map()(4));
            yield return new KeyValuePair<Key, object[]>(Key.NumPad5, mapper.Map()(5));
            yield return new KeyValuePair<Key, object[]>(Key.NumPad6, mapper.Map()(6));
            yield return new KeyValuePair<Key, object[]>(Key.NumPad7, mapper.Map()(7));
            yield return new KeyValuePair<Key, object[]>(Key.NumPad8, mapper.Map()(8));
            yield return new KeyValuePair<Key, object[]>(Key.NumPad9, mapper.Map()(9));
        }
    }
}
