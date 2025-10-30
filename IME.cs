﻿using System;
using VirtualKeyboard.Input.Interfaces;
using VirtualKeyboard.Input.Models;

namespace VirtualKeyboard.Input
{
    /// <summary>
    /// IME (Input Method Editor) - 입력 방식 관리 클래스
    /// </summary>
    public class IME
    {
        private readonly IKeyMapper _keyMapper;
        private readonly IInputComposer _composer;
        private CompositionContext _context;

        /// <summary>
        /// 조합 중인지 여부
        /// </summary>
        public bool IsComposing => _context.State.IsComposing;

        /// <summary>
        /// 조합기 이름
        /// </summary>
        public string ComposerName => _composer.Name;

        /// <summary>
        /// 키 매퍼 이름
        /// </summary>
        public string KeyMapperName => _keyMapper.Name;

        /// <summary>
        /// 조합기의 언어 코드
        /// </summary>
        public string Language => _composer.Language;

        /// <summary>
        /// IME 생성
        /// </summary>
        public IME(IKeyMapper keyMapper, IInputComposer composer)
        {
            _keyMapper = keyMapper ?? throw new ArgumentNullException(nameof(keyMapper));
            _composer = composer ?? throw new ArgumentNullException(nameof(composer));
            _context = new CompositionContext(_composer.CreateState());
        }

        /// <summary>
        /// 키 입력
        /// </summary>
        /// <param name="key">입력 문자 (예: 'a', 'A', 'r', 'R', '·', 'ㅡ' 등)</param>
        public CompositionResult Input(char key)
        {
            // 백스페이스는 항상 직접 처리
            if (key == '\b')
            {
                return Backspace();
            }

            // 특수 키는 Composer에게 먼저 위임
            if (IsSpecialKey(key))
            {
                var (handled, result) = _composer.TryProcessSpecialKey(_context, key);
                if (handled)
                {
                    return result;
                }

                // Composer가 처리하지 않으면 기본 동작
                return ProcessSpecialKeyDefault(key);
            }

            // 키 매핑
            if (_keyMapper.TryMap(key, out string mapped))
            {
                return ProcessMappedInput(mapped);
            }

            // 매핑되지 않은 문자는 NoChange 반환
            _context.State.Reset();
            return CompositionResult.NoChange();
        }

        /// <summary>
        /// 백스페이스
        /// </summary>
        public CompositionResult Backspace()
        {
            var result = _composer.ProcessBackspace(_context);

            if (result.Success)
            {
                return result;
            }

            // 조합 중이 아니면 NoChange 반환
            return CompositionResult.NoChange();
        }

        /// <summary>
        /// 조합을 완료하고 확정
        /// </summary>
        public CompositionResult Commit()
        {
            var result = _composer.Commit(_context);
            _context.State.Reset();
            return result;
        }

        /// <summary>
        /// 조합을 취소
        /// </summary>
        public CompositionResult Cancel()
        {
            var result = _composer.Cancel(_context);
            _context.State.Reset();
            return result;
        }

        /// <summary>
        /// 조합기 초기 상태로 리셋
        /// </summary>
        public void Reset()
        {
            _composer.Reset();
            _context.State.Reset();
        }

        /// <summary>
        /// 현재 컨텍스트의 복사본 반환
        /// </summary>
        public CompositionContext GetContext()
        {
            return _context.Clone();
        }

        /// <summary>
        /// 변환 후보 선택 (일본어/중국어 IME 등)
        /// </summary>
        /// <param name="candidateIndex">선택할 후보 인덱스</param>
        public CompositionResult SelectCandidate(int candidateIndex)
        {
            return _composer.SelectCandidate(_context, candidateIndex);
        }

        #region Private Methods

        /// <summary>
        /// 특수 키 여부 확인
        /// </summary>
        private bool IsSpecialKey(char key)
        {
            return key == '\n' || key == '\r' || key == ' ' || key == '\t' || key == '\x1b'; // ESC
        }

        /// <summary>
        /// 특수 키 기본 처리
        /// </summary>
        private CompositionResult ProcessSpecialKeyDefault(char key)
        {
            // 엔터: 조합 완료
            if (key == '\n' || key == '\r')
            {
                return Commit();
            }

            // 스페이스: 조합 완료
            if (key == ' ')
            {
                return Commit();
            }

            // ESC: 조합 취소
            if (key == '\x1b')
            {
                return Cancel();
            }

            // 기타 특수 키는 NoChange 반환
            return CompositionResult.NoChange();
        }

        /// <summary>
        /// 매핑된 입력 처리
        /// </summary>
        private CompositionResult ProcessMappedInput(string input)
        {
            var result = _composer.ProcessInput(_context, input);

            if (result.Success)
            {
                return result;
            }

            // 조합 실패 시 NoChange 반환
            _context.State.Reset();
            return CompositionResult.NoChange();
        }

        #endregion
    }
}

