# LVS_jiye — 탄막 로그라이크 모바일 게임 개발 규칙

## 프로젝트 개요

뱀파이어 서바이벌 장르의 탄막 로그라이크 모바일 게임.
- 엔진: Unity (URP, 모바일 최적화)
- 타겟 플랫폼: Android / iOS
- MCP for Unity 연동으로 Claude가 Unity 에디터를 직접 조작

---

## 아키텍처 원칙

### 폴더 구조 (Assets/)
```
Assets/
├── Scripts/
│   ├── Core/           # GameManager, SceneLoader, ServiceLocator
│   ├── Player/         # PlayerController, PlayerStats
│   ├── Enemy/          # EnemyBase, EnemySpawner, EnemyAI
│   ├── Weapon/         # WeaponBase, 개별 무기 클래스
│   ├── Projectile/     # ProjectileBase, 탄종별 클래스
│   ├── Roguelike/      # LevelUpSystem, UpgradeData, RunData
│   ├── UI/             # HUD, LevelUpUI, GameOverUI
│   ├── Data/           # ScriptableObject 정의
│   └── Utils/          # ObjectPool, ExtensionMethods, Helpers
├── Prefabs/
├── ScriptableObjects/
├── Scenes/
├── Sprites/
├── Audio/
└── MCPForUnity/            # 건드리지 않음
```

### 코드 패턴

- **ObjectPool** 필수 사용: 탄환·적·이펙트는 반드시 풀링. `Instantiate/Destroy` 직접 호출 금지
- **ScriptableObject 기반 데이터**: 무기·적·업그레이드 스탯은 SO로 분리. 코드에 하드코딩 금지
- **이벤트 기반 통신**: 시스템 간 직접 참조 대신 `UnityEvent` 또는 C# `Action` 사용
- **단일 책임**: 하나의 MonoBehaviour = 하나의 역할. 500줄 초과 시 분리 검토
- **인터페이스**: `IDamageable`, `IPoolable` 등 인터페이스로 결합도 낮추기

---

## 게임 시스템 설계

### 핵심 루프
1. 적 자동 스폰 → 플레이어 자동 공격 → 경험치 획득 → 레벨업 시 업그레이드 선택
2. 생존 시간이 점수. 30분 생존 시 최종 보스 등장

### 플레이어
- 조작: 터치 조이스틱 (가상 조이스틱, 단일 손가락)
- 공격: 완전 자동 (Vampire Survivors 방식)
- 스탯: HP, 이동속도, 공격력, 공속, 범위, 행운, 자석 범위

### 무기 시스템
- `WeaponBase` 추상 클래스 상속
- 각 무기는 SO(`WeaponData`)로 레벨별 스탯 정의
- 최대 6슬롯 장착

### 적 시스템
- `EnemySpawner`가 웨이브 SO를 읽어 스폰
- 적은 항상 플레이어를 향해 이동 (기본 AI)
- 엘리트·보스는 별도 행동 패턴

### 로그라이크 업그레이드
- 레벨업 시 3개 무작위 선택지 제시
- 선택지: 신규 무기 획득 / 기존 무기 강화 / 패시브 아이템
- 런 종료 시 초기화 (영구 해금은 메타 진행으로 별도 관리)

---

## Unity MCP 사용 규칙

- Unity 에디터가 열려 있어야 MCP 도구 사용 가능
- 씬 수정 후에는 반드시 `manage_scene`으로 저장 확인
- 스크립트 생성 시 `create_script` 사용, 경로는 `Assets/Scripts/` 하위
- 프리팹 생성·수정은 `manage_prefabs` 사용
- 컴파일 오류 확인은 `read_console` 사용

---

## 코딩 컨벤션

- 언어: C# (Unity 버전에 맞는 기능만 사용)
- 네이밍:
  - 클래스: `PascalCase`
  - 메서드: `PascalCase`
  - private 필드: `_camelCase`
  - public 프로퍼티: `PascalCase`
  - 상수: `ALL_CAPS`
- `[SerializeField]`로 Inspector 노출, `public` 필드 직접 노출 지양
- `using` 네임스페이스 최소화, 불필요한 `using` 제거
- 모든 `MonoBehaviour`에 `[RequireComponent]` 적극 활용

---

## 성능 규칙 (모바일 최적화)

- 탄환·적: 반드시 ObjectPool 사용
- `Update()`에서 `GetComponent<>()` 호출 금지 → `Awake()`/`Start()`에서 캐싱
- `FindObjectOfType()` 런타임 호출 금지 → ServiceLocator 또는 직접 참조
- 물리: 2D Physics 사용, Layer Matrix로 불필요한 충돌 체크 제거
- 텍스처: Sprite Atlas 사용, 최대 해상도 512x512 (UI 제외)
- 목표 프레임: 60fps (중급 Android 기준)

---

## 작업 흐름

1. 기능 구현 요청 → 설계 먼저 간단히 논의
2. ScriptableObject 데이터 구조 먼저 정의
3. 코어 로직 스크립트 작성
4. Unity MCP로 씬/프리팹에 연결
5. `read_console`로 오류 확인 후 수정

---

## 금지 사항

- `Destroy(gameObject)` 직접 호출 (풀로 반환할 것)
- 코드에 밸런스 수치 하드코딩 (SO 사용)
- `Camera.main` 반복 호출 (캐싱 필수)
- `string` 태그 비교 (`CompareTag()` 사용)
- 씬 간 데이터를 `PlayerPrefs`로 전달 (RunData SO 사용)
