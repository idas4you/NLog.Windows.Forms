# AsyncRichTextBoxTarget
[English](AsyncRichTextBoxTarget.md) | **한국어**

로그 메시지를 백그라운드 스레드에서 묶어 `RichTextBox` 컨트롤에 기록하는 타깃입니다.

`AsyncRichTextBoxTarget`은 [RichTextBoxTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/RichTextBoxTarget)을 확장한 타깃입니다. 로그 이벤트마다 컨트롤을 갱신하지 않고, 이벤트를 큐에 쌓아 두었다가 일정한 주기로 한꺼번에 기록합니다. 짧은 시간에 로그가 많이 발생해도 화면이 느려지지 않습니다.

## 언제 쓰나요
- 로그가 많아서 메시지마다 화면을 갱신하면 폼이 느려질 때는 `AsyncRichTextBoxTarget`을 씁니다.
- 메시지가 즉시 보여야 하거나, 메시지마다 행 색상·링크 처리가 따로 필요할 때는 `RichTextBoxTarget`을 씁니다.

## 설정

### NLog.config
이 패키지의 어셈블리 이름은 `NLog.Windows.Forms.Async`이고, 타깃 종류 이름은 `AsyncRichTextBox`입니다.

```xml
<nlog xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <extensions>
    <add assembly="NLog.Windows.Forms.Async"/>
  </extensions>

  <targets>
    <target name="rtb" xsi:type="AsyncRichTextBox"
            formName="MainForm"
            controlName="richTextBoxLog"
            layout="${longdate}|${level:uppercase=true}|${message}"
            interval="500" />
  </targets>

  <rules>
    <logger name="*" minlevel="Info" writeTo="rtb" />
  </rules>
</nlog>
```

### 코드
```csharp
NLog.LogManager.Setup().RegisterWindowsForms();

var richTextTarget = new NLog.Windows.Forms.AsyncRichTextBoxTarget
{
    Name = "rtb",
    FormName = "MainForm",
    ControlName = "richTextBoxLog",
    Layout = "${longdate}|${level:uppercase=true}|${message}",
    Interval = 500
};

NLog.LogManager.Setup().LoadConfiguration(cfg => cfg.ForLogger().WriteTo(richTextTarget));
```

`RegisterWindowsForms()`는 `AsyncRichTextBox` 종류 이름을 등록합니다. 설정 파일로 타깃을 만들 때만 필요합니다.

## 옵션

| 옵션 | 형식 | 기본값 | 설명 |
| --- | --- | --- | --- |
| `Interval` | `int` | `500` | 컨트롤에 기록하는 주기입니다. 단위는 밀리초입니다. |

- `Interval`을 작게 하면 메시지가 더 빨리 보입니다.
- `Interval`을 크게 하면 로그가 많을 때 컨트롤 갱신 횟수가 줄어듭니다.

그 밖의 옵션(`FormName`, `ControlName`, `Layout`, `MaxLines`, `AutoScroll`, 행·단어 색상 규칙, 메시지 보관 방식 등)은 모두 `RichTextBoxTarget`에서 물려받습니다. [RichTextBoxTarget 위키 문서](https://github.com/NLog/NLog.Windows.Forms/wiki/RichTextBoxTarget)를 참고하세요.

## 동작 방식
1. 로그 이벤트는 큐에 추가됩니다. 이때 컨트롤은 건드리지 않고 바로 반환합니다.
2. 첫 로그 이벤트가 들어오면 백그라운드 스레드가 시작됩니다.
3. 스레드는 `Interval` 밀리초마다 큐에 쌓인 이벤트를 모두 꺼내 각각 `Layout`으로 렌더링합니다.
4. 레벨이 같은 연속된 이벤트는 줄바꿈으로 이어 붙여 한 묶음으로 컨트롤에 기록합니다. 레벨이 바뀌면 새 묶음이 시작됩니다.

## RichTextBoxTarget과 다른 점
- **메시지가 늦게 보입니다.** 로그를 남긴 뒤 최대 `Interval` 밀리초가 지나야 화면에 나타날 수 있습니다.
- **행 색상 규칙은 이벤트 하나가 아니라 묶음 단위로 적용됩니다.** 묶음은 마지막 이벤트의 레벨과 로거 이름을 가지며, 메시지는 렌더링된 텍스트를 합친 것입니다. 레벨 기준 규칙은 그대로 동작합니다. 그 밖의 이벤트 정보(이벤트별 로거 이름, 예외, 이벤트 속성 등)를 쓰는 규칙은 묶음을 기준으로 평가됩니다.
- **종료 시 큐에 남은 메시지는 사라질 수 있습니다.** 애플리케이션이 닫힐 때 백그라운드 스레드가 큐를 비우지 않으므로, 마지막 주기 동안 남긴 로그는 컨트롤에 기록되지 않을 수 있습니다.
