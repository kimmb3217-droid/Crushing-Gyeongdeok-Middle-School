# 안동 경덕중학교 뿌시기

Solar Smash에서 영감을 받은 **가상의 3D 건물 파괴 샌드박스**입니다. 실제 학교의 정확한 복제품이 아닌, 학교를 연상시키는 단순화된 테스트 모델로 시작합니다.

## 목표 플랫폼
- **1차 목표: Android 휴대폰** — 친구들에게 APK 파일 또는 다운로드 페이지 링크로 배포
- Unity 프로젝트는 PC에서도 편집·테스트할 수 있도록 구성
- iPhone 배포는 별도 iOS 빌드와 Apple 서명/배포 절차가 필요하므로 후속 단계로 진행

## 현재 프로토타입
- 실행 시 학교 형태의 블록 건물과 지면 자동 생성
- PC: 마우스 클릭 공격, 우클릭 드래그 카메라 회전, 휠 줌
- 모바일: 화면 탭 공격, 드래그 카메라 회전, 두 손가락 핀치 줌
- 블록 체력, 파괴 시 물리 효과, 남은 블록 수 표시
- 외부 에셋이나 패키지 없이 기본 Unity 기능만 사용

## Unity에서 실행
1. Unity Hub에서 **Unity 2022.3 LTS 이상**의 3D 프로젝트를 만듭니다.
2. 이 저장소의 파일을 프로젝트 폴더로 복사합니다.
3. `Assets/Scripts/PrototypeBootstrap.cs`를 확인합니다.
4. 빈 씬에 빈 GameObject를 만들고 이름을 `PrototypeBootstrap`으로 지정합니다.
5. 스크립트를 해당 GameObject에 붙이고 Play를 누릅니다.

## Android APK 만들기
1. Unity Hub에서 사용 중인 Unity 버전에 맞는 **Android Build Support**, Android SDK & NDK Tools, OpenJDK 모듈을 설치합니다.
2. Unity에서 `File > Build Settings`(또는 최신 버전의 `Build Profiles`)를 엽니다.
3. Android를 선택하고 `Switch Platform`을 누릅니다.
4. Player Settings에서 Package Name을 예를 들어 `com.yourname.gyeongdeoksmash`로 설정합니다.
5. 씬을 빌드 목록에 추가하고 `Build`를 눌러 APK를 만듭니다.
6. APK를 친구들에게 전달하거나, 신뢰할 수 있는 다운로드 페이지에 올려 링크를 공유합니다. 친구들은 Android에서 APK 설치를 허용해야 할 수 있습니다. 출처가 불분명한 APK는 설치하지 않도록 안내하세요.

> Android APK는 Android 기기용입니다. iPhone에서는 APK가 실행되지 않습니다. iPhone 지원은 별도의 iOS 빌드와 배포가 필요합니다.

## 조작
- PC: 좌클릭 공격, 우클릭 드래그 회전, 마우스 휠 줌, R 재생성
- 모바일: 화면 탭 공격, 드래그 회전, 두 손가락 핀치 줌, 화면의 재생성 버튼

## 다음 단계
- 미사일, 레이저, 충격파 무기 선택 UI
- 파괴 단계 및 파편 최적화
- 터치 UI와 다양한 화면 비율 테스트
- 공개적으로 사용할 수 있는 참고 자료를 바탕으로 가상 건물 외형 개선

## 참고
이 프로젝트는 가상의 게임 시뮬레이션입니다. 현실의 학교나 사람에게 위해를 가하기 위한 용도가 아닙니다.
