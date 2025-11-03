# VirtualKeyboard.Input

다양한 언어의 입력 방식을 지원하는 IME(Input Method Editor) 라이브러리입니다.

## 개요

VirtualKeyboard.Input은 한글, 일본어, 중국어 등 복잡한 문자 조합이 필요한 언어의 입력을 처리하기 위한 확장 가능한 프레임워크입니다. 각 언어별 입력 방식을 `IInputComposer` 인터페이스로 구현하여 통일된 방식으로 사용할 수 있습니다.

## 주요 기능

- ✅ **다국어 입력 지원**: 한글, 일본어, 중국어 등 다양한 언어 입력 방식 구현 가능
- ✅ **조합 관리**: 문자 조합 중/확정 상태 관리
- ✅ **백스페이스 처리**: 조합 중인 문자의 단계별 백스페이스 처리
- ✅ **특수 키 처리**: Enter, Space, ESC 등 특수 키 처리
- ✅ **변환 후보**: 일본어/중국어 IME의 변환 후보 목록 지원
- ✅ **상태 관리**: 입력 상태의 저장/복원 가능

## 아키텍처

### 핵심 클래스

#### `IME`
입력 방식을 관리하는 메인 클래스입니다.

```csharp
public class IME
{
    // 조합 중인지 여부
    public bool IsComposing { get; }
    
    // 조합기 이름
    public string ComposerName { get; }
    
    // 언어 코드
    public string Language { get; }
    
    // 문자 입력
    public CompositionResult Input(char key);
    
    // 백스페이스
    public CompositionResult Backspace();
    
    // 조합 확정
    public CompositionResult Commit();
    
    // 조합 취소
    public CompositionResult Cancel();
    
    // 상태 초기화
    public void Reset();
}
```

#### `IInputComposer`
각 언어별 입력 조합기가 구현해야 하는 인터페이스입니다.

```csharp
public interface IInputComposer
{
    string Name { get; }
    string Language { get; }
    string Description { get; }
    
    CompositionResult ProcessInput(CompositionContext context, string input);
    CompositionResult ProcessBackspace(CompositionContext context);
    CompositionResult Commit(CompositionContext context);
    CompositionResult Cancel(CompositionContext context);
    
    ICompositionState CreateState();
    void Reset();
    bool CanProcess(string input);
    (bool handled, CompositionResult result) TryProcessSpecialKey(CompositionContext context, char key);
    CompositionResult SelectCandidate(CompositionContext context, int candidateIndex);
}
```

#### `CompositionResult`
조합 결과를 나타내는 불변 구조체입니다.

```csharp
public readonly struct CompositionResult
{
    public bool Success { get; }
    public string CommittedText { get; }      // 확정된 텍스트
    public string ComposingText { get; }      // 조합 중인 텍스트
    public string Buffer { get; }             // 조합 버퍼
    public string ErrorMessage { get; }       // 오류 메시지
    public ECompositionAction Action { get; }
    public IReadOnlyList<string> Candidates { get; }
    public int SelectedCandidateIndex { get; }
}
```

#### `ECompositionAction`
조합 작업 유형을 나타내는 열거형입니다.

```csharp
public enum ECompositionAction
{
    None,      // 작업 없음
    Input,     // 문자 입력
    Delete,    // 문자 삭제
    Commit,    // 조합 완료
    Cancel,    // 조합 취소
    Update     // 조합 업데이트
}
```

## 사용 방법

### 1. 기본 사용 예제

```csharp
using VirtualKeyboard.Input;
using VirtualKeyboard.Input.Korean.Composer;

// 한글 조합기 생성
var composer = new KoreanComposer();
var ime = new IME(composer);

// 문자 입력
var result1 = ime.Input('ㄱ');
Console.WriteLine($"조합 중: {result1.ComposingText}");  // "ㄱ"

var result2 = ime.Input('ㅏ');
Console.WriteLine($"조합 중: {result2.ComposingText}");  // "가"

// 조합 확정
var result3 = ime.Commit();
Console.WriteLine($"확정: {result3.CommittedText}");     // "가"
```

### 2. 실시간 텍스트 입력 처리

```csharp
using System.Text;

var composer = new KoreanComposer();
var ime = new IME(composer);
var textBuffer = new StringBuilder();
string composingText = "";

void ProcessKey(char key)
{
    var result = ime.Input(key);
    
    if (result.Success)
    {
        // 확정된 텍스트가 있으면 버퍼에 추가
        if (!string.IsNullOrEmpty(result.CommittedText))
        {
            textBuffer.Append(result.CommittedText);
        }
        
        // 조합 중인 텍스트 업데이트
        composingText = result.ComposingText;
        
        // UI 업데이트
        UpdateDisplay(textBuffer.ToString(), composingText);
    }
}

// 사용 예
ProcessKey('ㄱ');  // 조합: "ㄱ"
ProcessKey('ㅏ');  // 조합: "가"
ProcessKey('ㄴ');  // 확정: "간", 조합: ""
ProcessKey('ㅏ');  // 조합: "나"
ProcessKey('ㄷ');  // 확정: "나", 조합: "ㄷ"
```

### 3. 백스페이스 처리

```csharp
var ime = new IME(new KoreanComposer());

ime.Input('ㄱ');  // "ㄱ"
ime.Input('ㅏ');  // "가"
ime.Input('ㄴ');  // "간"

var result = ime.Backspace();
Console.WriteLine($"조합 중: {result.ComposingText}");  // "가"

result = ime.Backspace();
Console.WriteLine($"조합 중: {result.ComposingText}");  // "ㄱ"

result = ime.Backspace();
Console.WriteLine($"조합 없음: {ime.IsComposing}");     // false
```

### 4. 특수 키 처리

```csharp
var ime = new IME(new KoreanComposer());

ime.Input('ㄱ');
ime.Input('ㅏ');

// Enter로 확정
var result = ime.Input('\n');
Console.WriteLine($"확정: {result.CommittedText}");     // "가"

// 또는 Space로 확정
ime.Input('ㄴ');
result = ime.Input(' ');
Console.WriteLine($"확정: {result.CommittedText}");     // "ㄴ "

// ESC로 취소
ime.Input('ㄷ');
result = ime.Input('\x1b');  // ESC
Console.WriteLine($"조합 취소: {ime.IsComposing}");     // false
```

### 5. 상태 확인

```csharp
var ime = new IME(new KoreanComposer());

Console.WriteLine($"조합기 이름: {ime.ComposerName}");
Console.WriteLine($"언어: {ime.Language}");
Console.WriteLine($"조합 중: {ime.IsComposing}");

ime.Input('ㄱ');
Console.WriteLine($"조합 중: {ime.IsComposing}");  // true

ime.Commit();
Console.WriteLine($"조합 중: {ime.IsComposing}");  // false
```

### 6. CompositionResult 확장 메서드 활용

```csharp
using VirtualKeyboard.Input.Extensions;

var result = ime.Input('ㄱ');

// 텍스트 변경이 있는지 확인
if (result.HasTextChange())
{
    Console.WriteLine("텍스트가 변경되었습니다.");
}

// 조합 중인지 확인
if (result.IsComposing())
{
    Console.WriteLine($"조합 중: {result.ComposingText}");
}

// 버퍼가 있는지 확인
if (result.HasBuffer())
{
    Console.WriteLine($"버퍼: {result.Buffer}");
}
```

## 커스텀 Composer 구현

새로운 언어의 입력 방식을 구현하려면 `IInputComposer`를 구현하세요.

### 1. 상태 클래스 구현

```csharp
using VirtualKeyboard.Input.Abstracts;
using VirtualKeyboard.Input.Interfaces;

public class MyLanguageState : CompositionStateBase
{
    public string CurrentText { get; set; } = "";
    
    public override bool IsComposing => !string.IsNullOrEmpty(CurrentText);
    
    public override void Reset()
    {
        CurrentText = "";
    }
    
    public override ICompositionState Clone()
    {
        return new MyLanguageState { CurrentText = CurrentText };
    }
    
    public override string ToString()
    {
        return $"Text: {CurrentText}";
    }
}
```

### 2. Composer 구현

```csharp
using VirtualKeyboard.Input.Interfaces;
using VirtualKeyboard.Input.Models;

public class MyLanguageComposer : IInputComposer
{
    public string Name => "My Language Composer";
    public string Language => "xx-XX";
    public string Description => "My custom language input method";
    
    public ICompositionState CreateState() => new MyLanguageState();
    
    public bool CanProcess(string input)
    {
        // 처리 가능한 문자인지 확인
        return !string.IsNullOrEmpty(input);
    }
    
    public CompositionResult ProcessInput(CompositionContext context, string input)
    {
        var state = (MyLanguageState)context.State;
        
        // 입력 처리 로직 구현
        state.CurrentText += input;
        
        return CompositionResult.Succeeded(
            composingText: state.CurrentText,
            action: ECompositionAction.Input
        );
    }
    
    public CompositionResult ProcessBackspace(CompositionContext context)
    {
        var state = (MyLanguageState)context.State;
        
        if (string.IsNullOrEmpty(state.CurrentText))
            return CompositionResult.NoChange();
        
        state.CurrentText = state.CurrentText[..^1];
        
        return CompositionResult.Succeeded(
            composingText: state.CurrentText,
            action: ECompositionAction.Delete
        );
    }
    
    public CompositionResult Commit(CompositionContext context)
    {
        var state = (MyLanguageState)context.State;
        
        if (string.IsNullOrEmpty(state.CurrentText))
            return CompositionResult.NoChange();
        
        var text = state.CurrentText;
        state.Reset();
        
        return CompositionResult.Succeeded(
            composingText: "",
            committedText: text,
            action: ECompositionAction.Commit
        );
    }
    
    public CompositionResult Cancel(CompositionContext context)
    {
        var state = (MyLanguageState)context.State;
        state.Reset();
        
        return CompositionResult.Succeeded(
            composingText: "",
            action: ECompositionAction.Cancel
        );
    }
    
    public void Reset()
    {
        // 필요한 경우 Composer 자체의 상태 리셋
    }
    
    public (bool handled, CompositionResult result) TryProcessSpecialKey(
        CompositionContext context, char key)
    {
        // 특수 키를 직접 처리하려면 (handled: true, result) 반환
        // IME의 기본 처리를 사용하려면 (false, default) 반환
        return (false, default);
    }
    
    public CompositionResult SelectCandidate(CompositionContext context, int candidateIndex)
    {
        // 변환 후보 선택 로직 (일본어/중국어 IME 등)
        return CompositionResult.NoChange();
    }
}
```

### 3. 사용

```csharp
var composer = new MyLanguageComposer();
var ime = new IME(composer);

var result = ime.Input('a');
Console.WriteLine($"조합 중: {result.ComposingText}");
```

## 실제 구현 예제

### 한글 입력 (KoreanComposer)

한글 조합기는 별도의 `VirtualKeyboard.Input.Korean` 패키지로 제공됩니다.

```csharp
using VirtualKeyboard.Input;
using VirtualKeyboard.Input.Korean.Composer;

var composer = new KoreanComposer();
var ime = new IME(composer);

// "안녕하세요" 입력
ime.Input('ㅇ');
ime.Input('ㅏ');
ime.Input('ㄴ');  // "안" 확정
ime.Input('ㄴ');
ime.Input('ㅕ');
ime.Input('ㅇ');  // "녕" 확정
// ... 계속
```

## 관련 프로젝트

- `VirtualKeyboard.Input.Korean` - 한글 입력 조합기
- `VirtualKeyboard.Input.Japanese` - 일본어 입력 조합기 (예정)
- `VirtualKeyboard.Input.Chinese` - 중국어 입력 조합기 (예정)