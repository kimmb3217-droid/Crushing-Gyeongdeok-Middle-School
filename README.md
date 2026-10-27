# 안동 경덕중학교 뿌시기

Solar Smash에서 영감을 받은 **가상의 3D 건물 파괴 샌드박스**입니다. 실제 학교의 정확한 복제품이 아닌, 학교를 연상시키는 단순화된 테스트 모델로 시작합니다.

## 목표 플랫폼 및 배포
- Android: APK를 만들어 직접 공유하거나 다운로드 페이지에 올립니다.
- iPhone: macOS의 Xcode로 iOS 빌드를 만든 뒤 App Store Connect의 TestFlight로 테스트 배포합니다.
- Android APK는 iPhone에서 실행되지 않습니다. iOS 배포에는 Apple 개발자 계정/서명 절차가 필요할 수 있습니다.

## 현재 프로토타입
- 실행 시 학교 형태의 블록 건물과 지면 자동 생성
- PC: 좌클릭 공격, 우클릭 드래그 회전, 휠 줌
- 모바일: 탭 공격, 드래그 회전, 두 손가락 핀치 줌
- 레이저: 조준한 블록에 강한 단일 공격
- 미사일: 조준점 주변에 범위 피해
- 충격파: 더 넓은 범위 피해
- 건물 재생성 버튼, 남은 블록과 공격 횟수 표시
- 외부 에셋/패키지 없이 기본 Unity 기능 사용

## Unity에서 실행
1. Unity Hub에서 **Unity 2022.3 LTS 이상**의 3D 프로젝트를 만듭니다.
2. 이 저장소의 파일을 프로젝트 폴더로 복사합니다.
3. `Assets/Scripts/PrototypeBootstrap.cs`를 확인합니다.
4. 빈 씬에 빈 GameObject를 만들고 이름을 `PrototypeBootstrap`으로 지정합니다.
5. 스크립트를 해당 GameObject에 붙이고 Play를 누릅니다.

## 공통 Player Settings
- Product Name: `경덕중학교 뿌시기`
- Orientation: Landscape
- 모바일 테스트 전 화면 비율과 터치 조작을 확인합니다.

## Android APK 만들기
1. Unity Hub에서 Android Build Support, Android SDK & NDK Tools, OpenJDK를 설치합니다.
2. `File > Build Settings` 또는 `Build Profiles`에서 Android를 선택하고 Switch Platform을 누릅니다.
3. Package Name을 예: `com.yourname.gyeongdeoksmash`로 설정합니다.
4. 테스트용 APK 빌드를 선택하고 Build를 누릅니다.
5. APK를 신뢰할 수 있는 파일 공유/다운로드 링크로 친구에게 전달합니다. Android는 출처가 불분명한 APK를 설치하지 않도록 안내하세요.

## iPhone TestFlight 배포
1. Unity Hub에 iOS Build Support를 설치합니다.
2. Unity에서 iOS 플랫폼을 선택해 프로젝트를 빌드합니다.
3. macOS 컴퓨터에서 생성된 Xcode 프로젝트를 엽니다.
4. Apple 개발자 서명 설정 후 Xcode에서 Archive하여 App Store Connect에 업로드합니다.
5. App Store Connect의 TestFlight에서 테스트 빌드를 등록하고 친구를 테스터로 초대합니다.

iOS 빌드와 서명에는 macOS 및 Xcode 환경이 필요합니다. iOS 배포 요건과 계정 비용은 Apple의 최신 정책을 확인하세요.

## 조작
- PC: 좌클릭 공격, 우클릭 드래그 회전, 마우스 휠 줌, R 재생성
- 모바일: 탭 공격, 드래그 회전, 두 손가락 핀치 줌, 오른쪽 무기 버튼, 재생성 버튼

## 다음 단계
- UI를 Unity Canvas 기반으로 교체해 해상도/터치 입력 안정성 개선
- 파편 최적화, 사운드, 성능 테스트
- 공개적으로 사용할 수 있는 참고 자료를 바탕으로 가상 건물 외형 개선

## 참고
이 프로젝트는 가상의 게임 시뮬레이션입니다. 현실의 학교나 사람에게 위해를 가하기 위한 용도가 아닙니다.
