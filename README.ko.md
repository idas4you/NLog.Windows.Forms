# NLog.Windows.Forms
[English](README.md) | **한국어**

[![Version](https://badge.fury.io/nu/NLog.Windows.Forms.svg)](https://www.nuget.org/packages/NLog.Windows.Forms)
[![CI](https://github.com/NLog/NLog.Windows.Forms/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/NLog/NLog.Windows.Forms/actions/workflows/ci.yml)
[![codecov.io](https://codecov.io/github/NLog/NLog.Windows.Forms/coverage.svg?branch=master)](https://codecov.io/github/NLog/NLog.Windows.Forms?branch=master)

이 패키지는 Windows Forms 애플리케이션에 로그 출력을 표시하는 NLog 타깃을 제공합니다.

- [RichTextBoxTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/RichTextBoxTarget): 로그 메시지를 RichTextBox 컨트롤에 색상 강조와 함께 표시합니다.
- `AsyncRichTextBoxTarget`: 로그 메시지를 백그라운드에서 묶어 처리한 뒤 RichTextBox 컨트롤에 표시합니다.
- [MessageBoxTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/MessageBoxTarget): 로그 메시지를 메시지 상자로 표시합니다.
- [FormControlTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/FormControlTarget): 로그 메시지를 Windows Forms 컨트롤의 `Text` 속성에 표시합니다.
- [ToolStripItemTarget](https://github.com/NLog/NLog.Windows.Forms/wiki/ToolStripItemTarget): 로그 메시지를 `ToolStripItem`의 `Text` 속성에 표시합니다.

전체 [목록](https://nlog-project.org/config/?tab=targets&search=package:nlog.windows.forms)을 참고하세요.

## 확장 등록
[NLog.Windows.Forms](https://www.nuget.org/packages/NLog.Windows.Forms/) NuGet 패키지를 설치하고 [확장 어셈블리를 등록](https://github.com/NLog/NLog/wiki/Register-your-custom-component)합니다.

`NLog.config` 파일에서 설정을 불러오는 경우, `<extensions>` 섹션에 확장을 등록합니다:
```xml
<extensions>
  <add assembly="NLog.Windows.Forms"/>
</extensions>
```

또는 [Fluent 설정 API](https://github.com/NLog/NLog/wiki/Fluent-Configuration-API)를 사용해 코드에서 등록할 수도 있습니다:
```csharp
NLog.LogManager.Setup().RegisterWindowsForms();
```

## AsyncRichTextBoxTarget
`AsyncRichTextBoxTarget`은 `RichTextBoxTarget`을 확장한 타깃으로, 큐에 쌓인 로그 이벤트를 백그라운드 스레드에서 처리합니다.
메시지를 묶어서 대상 컨트롤에 한 번에 기록하므로 UI 갱신 부담을 줄일 수 있습니다.

예제 (코드로 설정):
```csharp
var richTextTarget = new NLog.Windows.Forms.AsyncRichTextBoxTarget
{
    Name = "rtb",
    FormName = "MainForm",
    ControlName = "richTextBoxLog",
    Layout = "${longdate}|${level:uppercase=true}|${message}",
    Interval = 500
};
```

- `Interval`은 처리 주기이며 단위는 밀리초입니다.
- 값을 작게 하면 UI에 더 빨리 반영됩니다.
- 값을 크게 하면 로그가 많을 때 갱신 횟수를 줄일 수 있습니다.
