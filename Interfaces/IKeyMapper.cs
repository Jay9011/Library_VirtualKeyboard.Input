using System.Collections.Generic;

namespace VirtualKeyboard.Input.Interfaces
{
    /// <summary>
    /// 키보드 키를 입력 문자로 매핑하는 인터페이스
    /// </summary>
    public interface IKeyMapper
    {
        /// <summary>
        /// 키 매핑의 이름
        /// 예: "QWERTY Korean", "CheonJiIn"
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 키 매핑의 설명
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 키를 문자로 매핑
        /// </summary>
        /// <param name="key">입력 키</param>
        /// <param name="mapped">매핑된 문자열</param>
        /// <returns>매핑 성공 여부</returns>
        bool TryMap(char key, out string mapped);

        /// <summary>
        /// 특정 키를 매핑할 수 있는지 확인
        /// </summary>
        /// <param name="key">확인할 키</param>
        /// <returns>매핑 가능 여부</returns>
        bool CanMap(char key);

        /// <summary>
        /// 지원하는 모든 키 목록 반환
        /// </summary>
        /// <returns>지원 키 목록</returns>
        IEnumerable<char> GetSupportedKeys();
    }
}
