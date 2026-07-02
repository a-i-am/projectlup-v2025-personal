# Project LUP

[한국어](#한국어) | [English](#english)

[A* 경로 탐색 영상](https://youtu.be/dFYgrskIBPk) | [오클루전 컬링 영상](https://youtu.be/smA8cWIU03Y) | [프로젝트 PDF](docs/projectlup.pdf)

## 한국어

공동 프레임워크와 제한된 리소스로 여러 장르를 제작한 13인 Unity 프로젝트입니다. 저는 SLG 팀에서 작업자를 운용해 벙커 시설을 관리하는 시스템을 개발했습니다.

README 업데이트: 2026-06-30

### 프로젝트 정보

| 항목 | 내용 |
| --- | --- |
| 개발 기간 | Upstream 저장소 생성일 2025-11-01 |
| 개인 리팩터링 기간 | 2025-11-12 - 2026-03-07 |
| 참여 인원 | 개발자 13인, 장르별 2-3인 |
| 담당 역할 | SLG 작업자 AI와 이동·편집 도구 |
| 엔진 | Unity 6000.0.62f1 |
| 플랫폼 | Android |

### 핵심 구현

- 행동 트리 우선순위에 따른 배회, 식사, 작업 이동
- Blackboard를 통한 작업자 상태와 행동 데이터 공유
- 2D 그리드 A* 경로 탐색과 건물 내부 PathPoint 이동 연결
- JSON 맵을 Scene View에서 배치하는 맵 에디터
- 건물 내부 이동 지점을 생성·수정하는 구조물 경로 에디터
- 동일 Mesh와 Material의 GPU Instancing 검사 도구
- 오클루전 컬링 설정과 렌더링 상태 검증



### 성능·리팩터링

- GPU Instancing 적용 전후 Batches를 `1,615 -> 961`로 측정해 약 40.5% 감소를 확인했습니다.
- 가려진 오브젝트의 불필요한 렌더링을 줄이기 위해 오클루전 컬링을 적용했습니다.
- 이동 행동의 공통 흐름, 행동 트리 노드, Blackboard 키와 작업자 참조 구조를 정리했습니다.
- 맵·구조물 편집 도구를 런타임 데이터 구조와 분리했습니다.

### 기술 스택
<p>
  <img src="https://img.shields.io/badge/Unity 6-000000?style=flat-square&logo=unity&logoColor=white"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white"/>
  <img src="https://img.shields.io/badge/JSON-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/A*-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/Behaviour Tree-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/GPU Instancing-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/Occlusion Culling-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/Git-F05032?style=flat-square&logo=git&logoColor=white"/>
  <img src="https://img.shields.io/badge/Fork-000000?style=flat-square"/>
</p>

### 에셋 및 원본

- 공동 프레임워크와 프로젝트 에셋: Project LUP 팀 upstream
- Unity 패키지와 외부 에셋: 저장소에 포함된 원본 라이선스와 배포 조건 적용
- 재배포 조건을 확인할 수 없는 에셋은 공개 포트폴리오에서 제외

### 배운 점

다수의 개발자가 같은 프레임워크를 수정할 때는 기능 구현뿐 아니라 데이터 소유권, 브랜치 동기화, 편집 도구와 런타임 코드의 경계를 명확히 해야 충돌과 회귀를 줄일 수 있었습니다.

### 브랜치 및 커밋 정리

- 리팩터링 커밋은 `refactor` / `refactoring` 키워드 기준으로 구분했습니다.
- 공개용 문서는 기능 구현과 리팩터링 근거가 같은 README에서 바로 보이도록 정리했습니다.

### 업데이트 계획

- 필요 시 편집 도구와 성능 측정 화면을 영상 링크로 추가합니다.
- 공개 배포 전에는 외부 에셋의 재배포 조건을 다시 확인합니다.

## English

Project LUP is a 13-programmer Unity project where genre teams built games on a shared framework with limited resources. I worked on the SLG team and implemented worker AI, navigation, and editor tools for bunker management.

Last updated: 2026-06-30

### Project

| Item | Details |
| --- | --- |
| Development | Upstream repository created 2025-11-01 |
| Personal refactoring | 2025-11-12 - 2026-03-07 |
| Team | 13 programmers, two or three per genre team |
| Role | SLG worker AI, navigation, and editor tooling |
| Engine | Unity 6000.0.62f1 |
| Platform | Android |

### Implementation

- Priority-based worker behavior for idling, eating, and assigned work
- Shared worker state through a Blackboard
- Grid A* navigation connected to editable indoor PathPoints
- Scene View tools for JSON map placement and indoor routes
- GPU Instancing measurement and occlusion-culling validation

Behavior conditions and actions were separated so new priorities did not require rewriting a large state-transition block. Dynamic outdoor paths use A*, while authored indoor movement uses editable PathPoints.

### Refactoring and Performance

- Measured Batches decreasing from `1,615` to `961`, approximately 40.5%, after GPU Instancing changes
- Reduced hidden-object rendering with occlusion culling
- Reorganized shared movement flow, behavior-tree nodes, Blackboard keys, and worker references
- Kept editor tooling separate from runtime data handling

### Stack and Assets
<p>
  <img src="https://img.shields.io/badge/Unity 6-000000?style=flat-square&logo=unity&logoColor=white"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white"/>
  <img src="https://img.shields.io/badge/JSON-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/A*-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/Behaviour Tree-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/GPU Instancing-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/Occlusion Culling-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/Git-F05032?style=flat-square&logo=git&logoColor=white"/>
  <img src="https://img.shields.io/badge/Fork-000000?style=flat-square"/>
</p>

The shared framework and project assets come from the Project LUP upstream team repository. Included packages and third-party assets remain subject to their original licenses; material without verified redistribution terms is excluded from the public portfolio.

### Lessons

In a shared framework, clear data ownership, branch synchronization, and boundaries between editor tools and runtime code are as important as the feature itself.

### Branch and Commit Notes

- Refactoring commits are grouped by the `refactor` / `refactoring` keywords.
- The README keeps feature scope and refactoring rationale together for easier portfolio review.

### Update Plan

- Add video links for the editor tooling and performance validation if needed.
- Recheck redistribution terms for external assets before public release.
