# LVS_jiye Unity 프로젝트 규칙

## 프로젝트 개요

뱀파이어 서바이벌 장르의 탄막 로그라이크 모바일 게임.
- 엔진: Unity (URP, 모바일 최적화)
- 타겟 플랫폼: Android / iOS
- MCP for Unity 연동으로 Claude가 Unity 에디터를 직접 조작

---

## 파일 및 폴더 구조

- 모든 C# 스크립트는 `Assets/Scripts/` 하위의 기능별 폴더에 생성한다.
- 해당 폴더가 없으면 새로 만든다.

```
Assets/
├── Scripts/
│   ├── Core/          # GameManager, SceneLoader 등 핵심 시스템
│   ├── Player/        # 플레이어 컨트롤러, 스탯
│   ├── Enemy/         # 적 AI, 스포너
│   ├── Weapon/        # 무기 베이스 및 개별 무기
│   ├── Projectile/    # 탄환 클래스
│   ├── Roguelike/     # 레벨업, 업그레이드, 런 데이터
│   ├── UI/            # HUD, 메뉴, 팝업
│   ├── Data/          # ScriptableObject 정의
│   ├── SO/
│   │   └── Skill/     # SkillData ScriptableObject 에셋
│   └── Utils/         # 유틸리티, 확장 메서드, ObjectPool
├── Prefabs/
├── Resources/
│   └── Prefabs/
│       └── Skill/     # 스킬 관련 프리팹
├── ScriptableObjects/
├── Scenes/
├── Sprites/
├── Audio/
└── MCPForUnity/       # 건드리지 않음
```

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
- 프리팹 생성·수정은 `manage_prefabs` 사용
- 컴파일 오류 확인은 `read_console` 사용

### MCP vs Claude Code 툴 구분
- **C# 스크립트 읽기/생성/수정** → Claude Code의 `Read`, `Write`, `Edit` 툴 사용 (MCP 불필요)
- **Unity 에디터 조작** (씬 저장, 프리팹, 게임오브젝트, 콘솔 확인 등) → MCP for Unity 사용

---

## 네이밍 컨벤션

### 전역(필드) 변수
- 언더스코어(`_`) + 카멜케이스
- 두 단어 이상은 두 번째 단어부터 첫 글자 대문자

```csharp
// 좋은 예
private int _score;
private float _moveSpeed;
private bool _isAlive;
private GameObject _playerObject;

// 나쁜 예
private int score;
private float MoveSpeed;
private bool IsAlive;
```

### 함수(메서드)
- 파스칼케이스 사용

```csharp
// 좋은 예
public void TakeDamage(float amount) { }
private void SpawnEnemy() { }
protected void OnPlayerDeath() { }

// 나쁜 예
public void takeDamage(float amount) { }
private void spawn_enemy() { }
```

### 그 외 규칙
| 대상 | 표기법 | 예시 |
|------|--------|------|
| 클래스 | PascalCase | `EnemySpawner` |
| public 프로퍼티 | PascalCase | `MoveSpeed` |
| 로컬 변수 | camelCase | `tempValue` |
| 상수 | ALL_CAPS | `MAX_ENEMY_COUNT` |
| 인터페이스 | I + PascalCase | `IDamageable` |

---

## 코드 규칙

### namespace
- `namespace` 사용 금지 — 모든 클래스는 전역 네임스페이스에 작성

### Manager 하위 클래스 설계 기준

Manager에 등록하는 기능별 클래스는 MonoBehaviour 필요 여부에 따라 두 가지로 구분한다.

**MonoBehaviour 불필요** (Unity 라이프사이클, Inspector 직렬화 불필요)
- 순수 C# 클래스로 작성
- Manager에서 `new`로 직접 생성

```csharp
// UIManager.cs
public class UIManager
{
    public VirtualJoystick Joystick { get; private set; }
    public void RegisterJoystick(VirtualJoystick joystick) { Joystick = joystick; }
}

// Manager.cs
public UIManager UI = new UIManager();
```

**MonoBehaviour 필요** (Awake/Update/코루틴/Inspector 직렬화 필요)
- MonoBehaviour 상속
- Manager 오브젝트의 자식으로 씬에 배치
- Manager의 `InitManagers()`에서 `GetComponentInChildren<>()`으로 참조

```csharp
// SomeManager.cs
public class SomeManager : MonoBehaviour { ... }

// Manager.cs
public SomeManager Some { get; private set; }
private void InitManagers()
{
    Some = GetComponentInChildren<SomeManager>();
}
```

### 코드 배치 순서
- 변수(필드)는 항상 함수(메서드) 위에 위치

```csharp
// 좋은 예
public class Enemy : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    private Rigidbody2D _rb;

    private void Awake() { _rb = GetComponent<Rigidbody2D>(); }
    private void Update() { ... }
}

// 나쁜 예
public class Enemy : MonoBehaviour
{
    private void Awake() { ... }

    private float _moveSpeed;  // 함수 아래 변수 선언
}
```

### [SerializeField]
- 인스펙터에 노출할 필드는 `public` 대신 `private [SerializeField]` 사용

```csharp
// 좋은 예
[SerializeField] private float _moveSpeed;
[SerializeField] private GameObject _bulletPrefab;

// 나쁜 예
public float moveSpeed;
public GameObject bulletPrefab;
```

### GetComponent 캐싱
- `Update()`에서 `GetComponent<>()` 호출 금지
- 반드시 `Awake()` 또는 `Start()`에서 캐싱 후 사용

```csharp
// 좋은 예
private Rigidbody2D _rb;
private void Awake() { _rb = GetComponent<Rigidbody2D>(); }
private void Update() { _rb.velocity = ...; }

// 나쁜 예
private void Update() { GetComponent<Rigidbody2D>().velocity = ...; }
```

### ObjectPool 필수 사용
- **탄환, 적, 이펙트**는 `Instantiate()` / `Destroy()` 직접 호출 금지
- 반드시 ObjectPool을 통해 생성·반환

```csharp
// 좋은 예
ObjectPool.Instance.Get("Bullet");
ObjectPool.Instance.Return("Bullet", bulletObj);

// 나쁜 예
Instantiate(bulletPrefab);
Destroy(gameObject);
```

### ScriptableObject 데이터 분리
- 무기·적·업그레이드의 수치(속도, 데미지, 쿨타임 등)는 ScriptableObject로 정의
- 코드 내 밸런스 수치 하드코딩 금지

```csharp
// 좋은 예
[SerializeField] private WeaponData _weaponData;
float damage = _weaponData.damage;

// 나쁜 예
float damage = 25.0f;
```

---

## 성능 규칙 (모바일 최적화)

- 탄환·적: 반드시 ObjectPool 사용
- `Update()`에서 `GetComponent<>()` 호출 금지 → `Awake()`/`Start()`에서 캐싱
- `FindObjectOfType()` 런타임 호출 금지 → 직접 참조 사용
- `Camera.main` 반복 호출 금지 → `Awake()`에서 캐싱
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
- `FindObjectOfType()` 런타임 호출
