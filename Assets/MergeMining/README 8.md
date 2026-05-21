# Iteration 11 — Procedural Audio + Grid Save

## 1. Что было добавлено
Процедурно генерируемые звуки на все действия (AudioClip.Create + PCM-сэмплы, sin/square/triangle/saw волны + envelope). Сохранение состояния сетки кирок в PlayerPrefs в виде JSON. Загрузка сетки при запуске сцены.

## 2. Что изменилось с прошлой итерации

### Звуки:
- Создан `ProceduralAudio` static class — генерация чистых тонов, sweep'ов (linear/exponential), белого шума с low-pass, аккордов, арпеджио. 4 типа волн: Sine, Square, Triangle, Saw
- Создан `SfxLibrary` singleton (на DontDestroyOnLoad) — генерирует 15 готовых AudioClip'ов на Awake: uiClick, shopBuy, pickaxePickup/Drop, merge, miningHit, blockExplode, coinTick, zoneComplete, chestOpen, boosterActivate, achievementUnlock, dailyReward, popupOpen/Close
- Подключены звуки в:
  - `ButtonAnimator` — uiClick на каждое нажатие кнопки (если не задан AudioClip явно)
  - `ShopController` — shopBuy при покупке
  - `PickaxeDragHandler` — pickaxePickup на BeginDrag, pickaxeDrop на drop в пустой слот
  - `PickaxeGridManager` — merge при успешном merge
  - `MiningAttack` — miningHit при попадании (с random pitch 0.85-1.15 для variety)
  - `BlocksRowManager` — blockExplode при разрушении, coinTick на каждой монете прилетающей в HUD
  - `CurrencyFlyEffect` — coinTick на завершение полёта
  - `BasePopup` — popupOpen на Show, popupClose на Hide
  - `ZoneCompletePopup` — zoneComplete fanfare
  - `ChestOpeningPopup` — chestOpen sweep
  - `BoosterManager` — boosterActivate на активацию любого бустера
  - `AchievementToast` — achievementUnlock на появление toast'а
  - `DailyRewardPopup` — dailyReward ascending tones

### Сохранение сетки:
- Создан `GridSaveData` — простой serializable список (row, col, level)
- `PickaxeGridManager.SaveGrid()` — собирает все кирки и записывает JSON в PlayerPrefs `grid_save_json`
- `PickaxeGridManager.TryLoadGrid()` — на Start читает JSON, восстанавливает кирки в нужных слотах
- SaveGrid вызывается после:
  - `AddPickaxe` (покупка/подарок)
  - `MovePickaxeToSlot` (drag в пустой слот)
  - `DoMerge` (после анимации завершения)
  - `OnApplicationPause(true)` — при сворачивании приложения
  - `OnApplicationQuit` — при выходе

### Логика старта:
- Если есть сохранённая сетка → восстанавливаем её
- Если нет сохранения и сетка пустая → даём 2 стартовые кирки lvl 1 (даже если starter_pickaxes_given=1 — это работа починки если игрок потерял прогресс)

## 3. Editor скрипты

`Tools / Merge Mining / (Iteration 11) Update ALL Scenes`

## 4. Как тестировать

### Звуки:
1. Открой Game scene → Play → должны идти звуки на каждое действие
2. Нажать любую кнопку → "пик" UI click
3. Купить кирку → восходящая 3-нотная мелодия
4. Зажать кирку → "пип" pickup
5. Положить в пустой слот → "поп" drop
6. Сделать merge → 4-нотный аккорд (восходящий)
7. Кирка попала по блоку → шумовой удар (с разной высотой)
8. Блок разрушился → "взрыв" шум
9. Монеты прилетают в HUD → "тики"
10. Pause popup открывается → восходящий sine sweep
11. Открыть Free Chest → "магический" sweep
12. Активировать бустер → быстрый saw sweep
13. Получить ачивку → восходящая мелодия из 4 нот
14. Daily Reward popup → длинная мелодия из 6 нот

### Save кирок:
15. Купи 5 кирок, сделай несколько merge → выйди из Play режима
16. Снова войди в Play → сетка должна выглядеть **точно так же** как до выхода (все купленные кирки на тех же слотах, все merged уровни сохранены)
17. Сделай ещё merge, выйди — войди — состояние сохранено
18. RESET PROGRESS → сетка очищается, при следующем запуске даются 2 стартовые кирки lvl 1

### Громкость:
19. Settings → SFX toggle → выкл — звуков нет
20. Settings → SFX slider — громкость регулируется

## 5. Ожидаемый результат
- Звуки звучат как ретро 8-бит из-за square/triangle волн
- Звуки не перегружают (envelope с быстрым attack и decay)
- При множественных одновременных событиях (например 6 монет одновременно) — звуки накладываются, но не перегружают
- Сохранение сетки работает мгновенно — изменение → сохранение → можно выйти

## 6. Известные ограничения
- Все звуки генерируются на старте приложения в SfxLibrary.Awake() — это занимает ~50-100ms на современных устройствах. Если будут заметные лаги — можно вынести в Coroutine с yield return null между clip'ами
- Громкости звуков подобраны "на глаз" — точечная балансировка через volumeScale в отдельных вызовах
- Звуки моно (1 канал) — для SFX это норма

## 7. Game is now truly complete
Все исходные требования ТЗ выполнены + звуки на все действия + сохранение прогресса полное.
