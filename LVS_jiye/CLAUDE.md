# LVS_jiye Unity 프로젝트 규칙

## 파일 및 폴더 구조

- 모든 C# 스크립트는 `Assets/Scripts/` 하위의 기능별 폴더에 생성한다.
- 해당 폴더가 없으면 새로 만든다.

```
Assets/Scripts/
├── Core/          # GameManager, SceneLoader 등 핵심 시스템
├── Player/        # 플레이어 컨트롤러, 스탯
├── Enemy/         # 적 AI, 스포너
├── Weapon/        # 무기 베이스 및 개별 무기
├── Projectile/    # 탄환 클래스
├── Roguelike/     # 레벨업, 업그레이드, 런 데이터
├── UI/            # HUD, 메뉴, 팝업
├── Data/          # ScriptableObject 정의
└── Utils/         # 유틸리티, 확장 메서드, ObjectPool
```

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
