# SharedProperties-Unity

## Описание

Компонент-хранилище общих для нескольких скриптов значений с доступом по ключу. Хотя использование данного компонента является нарушением принципа ООП "Инкапсуляция", при правильном и не частом применении, такой компонент может помочь решить некоторые задачи.

Логика работы компонента-хранилища чем-то похожа на Dictionary, данные в ней хранятся в виде пар ключ-значение, и так же является Generic типом. Однако доступ к значениям можно получить только по ID, который генерируется для каждого нового ключа. Таким образом отпадают вычисления хеш-таблиц, доступ к значениям быстрее и практически равен O(1). Так же компоненты практически не производят аллокаций памяти в рантайме, даже если запросить доступ к значению, к которому ранее не обращались, оно уже будет инициализировано заранее.

## Компоненты-хранилища
В расширении имеется 2 основных класса `SharedClassProperties<TKey, TValue>` и `SharedValueProperties<TKey, TValue>`, первый в качестве `TValue` принимает ссылочные типы, другой значимые типы, в качестве `TKey` может выступать любой тип.

Для добавления компонента к объекту, необходимо создать класс-наследник, как в примерах:
```C# 
using DCFApixels;
public class SharedPropertiesExample : SharedValueProperties<string, float> { }
```
```C# 
using DCFApixels;
public class SharedPropertiesExample : SharedClassProperties<string, Foo> { }
public class Foo {...}
```
## Общие значений
Доступ к общим значениям можно получить с помощью метода `SharedProperties.Get(int id)` который вернет `ref TValue` для значимых типов и `TValue` для ссылочных.

ID ключей для доступа к значениям, рекомендуется записывать в статические поля, а метод `SharedProperties.GetId(TKey key)` не вызывать в `Update`. Примеры рекомендуемых способов использования общих значений внутри скриптов:

Первый способ (Кэширование ссылки на SharedProperties в сериализуемое поле)
```C# 
    [RequireComponent(typeof(SharedPropertiesTest))]
    public class MonoBehaviourExample : MonoBehaviour
    {
        [SerializeField] private SharedPropertiesExample _shared;
        private static int speedID = SharedPropertiesExample.GetId("Speed");
        private ref float Speed => ref _shared.Get(speedID);
        private void OnValidate()
        {
            _shared = GetComponent<SharedPropertiesTest>();
        }
        private void Update()
        {
            transform.position += transform.forward * Speed;
            Speed += 0.1f;
        }
    }
```
Второй способ (Кэширование ссылки на SharedProperties в рантайме)
```C# 
    [RequireComponent(typeof(SharedPropertiesTest))]
    public class MonoBehaviourExample : MonoBehaviour
    {
        [SerializeField] private SharedPropertiesExample _shared;
        private static int speedID = SharedPropertiesExample.GetId("Speed");
        private ref float Speed => ref _shared.Get(speedID);
        private void Awake()
        {
            _shared = GetComponent<SharedPropertiesTest>();
        }
        private void Update()
        {
            transform.position += transform.forward * Speed;
            Speed += 0.1f;
        }
    }
```

## Инициализация значений

Все наследники `SharedClassProperties<TKey, TValue>` и `SharedValueProperties<TKey, TValue>` в инспекторе имеют массив `Pairs` - это и есть хранилище всех значений. В рантайме, отсутствующие значения автоматически добавляются в `Pairs`. Для предварительной инициализации значения его нужно добавить в этот массив вручную.

![image](https://user-images.githubusercontent.com/99481254/192299767-0e286cb2-93ec-428b-b98a-1b250e7797b3.png)

Важно! Стоит учитывать что из-за особенности работы этого расширения, компоненты-хранилища создают значения для каждого ключа, который был указан в `SharedProperties.GetId(TKey key)`, из-за чего некоторые игровые объекты могут потреблять избыточную память, но в большинстве случаев это не будет проблемой.
