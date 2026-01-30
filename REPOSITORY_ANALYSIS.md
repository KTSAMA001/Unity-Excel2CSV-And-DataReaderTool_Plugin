# Unity-Excel2CSV-And-DataReaderTool 仓库功能全面分析

## 项目概述

Unity-Excel2CSV-And-DataReaderTool 是一个专为 Unity 游戏引擎设计的数据管理插件工具。该工具的核心功能是将 Excel 表格（.xlsx 格式）转换为 CSV 文件，并自动生成 C# 脚本代码，使开发者能够在 Unity 中快速、便捷地读取和使用游戏数据。

### 主要优势

1. **快速数据修改**：直接修改 Excel 表格，重新生成即可，无需手动编写数据读取代码
2. **自动化代码生成**：自动生成类型安全的 C# 数据访问类
3. **简化工作流程**：统一的数据管理方式，降低了数据配置的复杂度
4. **支持多语言**：内置多语言数据支持，方便游戏国际化

---

## 核心功能模块

### 1. Excel 到 CSV 转换器（ExcelToCSVConverterEditor.cs）

**位置**：`Assets/Excel2CSV/ExcelToCSVConverterEditor.cs`

**功能描述**：
这是整个插件的核心模块，提供了 Unity 编辑器扩展功能，用于将 Excel 文件转换为 CSV 文件。

**主要特性**：

#### 1.1 Unity 编辑器菜单集成
- 在 Unity 编辑器菜单栏添加了 "KT CSV Tools" 菜单
- 提供两个主要功能：
  - `Convert Excel to CSV`：将 Excel 文件转换为 CSV 并生成 C# 脚本
  - `Delete All file`：删除所有生成的 CSV 和 C# 脚本文件

#### 1.2 Excel 数据解析
- 使用 ExcelDataReader 库读取 .xlsx 格式的 Excel 文件
- 支持自动识别表头（第一行作为列名）
- 智能处理特殊标记：
  - 使用 `{}` 包裹的内容会被自动过滤，不会被写入 CSV 文件
  - 支持在列名和数据中添加注释和说明

#### 1.3 数据清洗和格式化
- 自动移除 `{}` 包裹的注释内容
- 清除换行符，保证 CSV 格式正确
- 保留实际的数据内容

#### 1.4 自动化 C# 脚本生成
- 为每个 CSV 文件自动生成对应的 C# 类
- 生成的类命名规则：`[Excel文件名]CSV`（如 Hero.xlsx → HeroCSV.cs）
- 自动创建数据加载类：`[类名]Load`（如 HeroCSVLoad）

**关键代码功能**：
```csharp
// 转换逻辑
[MenuItem("KT CSV Tools/Convert Excel to CSV", priority = 5)]
public static void ConvertExcelToCSV()
```

**工作流程**：
1. 扫描 `Assets/Excel2CSV/Excel` 目录下的所有 .xlsx 文件
2. 使用 ExcelDataReader 读取 Excel 数据
3. 处理列名，移除 `{}` 标记的注释
4. 将数据写入 `Assets/Excel2CSV/Resources/CSV` 目录
5. 读取生成的 CSV 文件
6. 自动生成 C# 数据访问类到 `Assets/Excel2CSV/ScriptsCS` 目录
7. 刷新 Unity 资源数据库

---

### 2. CSV 数据读取器（CSVReader.cs）

**位置**：`Assets/Excel2CSV/CSVReader.cs`

**功能描述**：
提供了一个静态工具类，用于在运行时动态读取 CSV 数据。

**核心方法**：

#### 2.1 ReadDataRow 方法
```csharp
public static object ReadDataRow(string typeName, string id, string key)
```

**参数说明**：
- `typeName`：CSV 类型名称（如 "HeroCSV"）
- `id`：要查找的数据行的 ID 值
- `key`：要获取的数据字段名称

**工作原理**：
1. 使用反射机制动态获取类型
2. 调用对应的 Load 方法加载指定 ID 的数据行
3. 通过属性反射获取指定字段的值
4. 返回查询结果

**优势**：
- 通用的数据读取接口
- 支持动态类型和字段访问
- 错误处理和日志记录

**使用示例**：
```csharp
// 读取 Hero 表中 ID 为 "1" 的数据的 Name 字段
string heroName = (string)CSVReader.ReadDataRow("HeroCSV", "1", "Name");
```

---

### 3. 自动生成的 C# 数据类

**位置**：`Assets/Excel2CSV/ScriptsCS/`

**功能描述**：
系统会为每个 CSV 文件自动生成两个类：

#### 3.1 数据类（如 HeroCSV）
- 继承自 `CSVBase` 基类
- 包含与 CSV 列对应的属性
- 所有属性都是字符串类型，带有 get 和 set 访问器

**示例**（HeroCSV.cs）：
```csharp
public class HeroCSV : CSVBase
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string SKILL { get; set; }
    public string TestData { get; set; }
    public string TestData2 { get; set; }
}
```

#### 3.2 数据加载类（如 HeroCSVLoad）
- 提供静态的 Load 方法
- 通过 Resources.Load 加载 CSV 文件
- 解析 CSV 数据并填充数据对象
- 使用 ID 作为主键查找数据行

**工作流程**：
1. 从 Resources 文件夹加载 CSV TextAsset
2. 将 CSV 文本按行分割
3. 遍历数据行，查找匹配的 ID
4. 将匹配行的数据填充到对象属性中
5. 返回填充好的数据对象

**示例**（HeroCSVLoad.cs）：
```csharp
public class HeroCSVLoad
{
    public static HeroCSV herocsv = new HeroCSV();
    static string filePath = "CSV/Hero";
    
    public static HeroCSV Load(string id)
    {
        var csvTextAsset = Resources.Load<TextAsset>(filePath);
        var csvData = csvTextAsset.text;
        var csvRows = csvData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 1; i < csvRows.Length; i++)
        {
            var row = csvRows[i].Split(',');
            if (row[0] == id)
            {
                herocsv.ID = row[0];
                herocsv.Name = row[1];
                herocsv.SKILL = row[2];
                herocsv.TestData = row[3];
                herocsv.TestData2 row[4];
                break;
            }
        }
        return herocsv;
    }
}
```

---

### 4. 基础类（CSVBase.cs）

**位置**：`Assets/Excel2CSV/CSVBase.cs`

**功能描述**：
所有自动生成的 CSV 数据类的基类。目前是一个空实现，为未来的扩展预留了空间。

**命名空间**：`CSV_SPACE`

**设计意图**：
- 统一数据类的继承结构
- 便于后续添加通用功能
- 支持多态和类型检查

---

### 5. 第三方库：ExcelDataReader

**位置**：`Assets/Excel2CSV/Plugins/ExcelDataReader-develop/`

**功能描述**：
这是一个轻量级的 C# Excel 文件读取库，用于解析 Excel 文件。

**支持的格式**：
- .xlsx（Excel 2007 及更高版本）
- .xlsb（Excel 2007 及更高版本）
- .xls（Excel 97-2003）
- 其他 BIFF 格式（Excel 2.0 到 Excel 95）

**在项目中的作用**：
- 提供核心的 Excel 文件解析能力
- 支持读取 Excel 的数据集
- 处理不同版本的 Excel 文件格式

---

## 目录结构详解

```
Assets/Excel2CSV/
├── Excel/                    # Excel 表格存放目录（输入）
│   ├── Hero.xlsx            # 英雄数据表
│   ├── Item.xlsx            # 物品数据表
│   └── Lan.xlsx             # 多语言数据表
│
├── Resources/CSV/           # 生成的 CSV 文件目录（中间产物）
│   ├── Hero.csv
│   ├── Item.csv
│   └── Lan.csv
│
├── ScriptsCS/               # 自动生成的 C# 脚本目录（输出）
│   ├── HeroCSV.cs
│   ├── ItemCSV.cs
│   └── LanCSV.cs
│
├── Plugins/                 # 第三方插件目录
│   └── ExcelDataReader-develop/   # Excel 读取库
│
├── CSVBase.cs               # CSV 数据类基类
├── CSVReader.cs             # CSV 数据读取器
├── ExcelToCSVConverterEditor.cs   # 编辑器转换工具
├── Test.cs                  # 示例测试脚本
└── CSVTool.asmdef          # 程序集定义文件
```

---

## 数据表格规则和约定

### 1. Excel 表格结构规则

#### 1.1 列名规则
- **第一行必须是列名行**：这一行将被用作生成 C# 类的属性名
- **列名要求**：建议使用英文字母，首字母会被自动转换为大写

#### 1.2 数据ID规则
- **第一列必须是 ID 列**：用作数据查询的主键
- **ID 必须唯一**：每行的 ID 应该是唯一的，用于定位数据

#### 1.3 注释和备注规则
使用 `{}` 标记注释内容，这些内容不会被写入 CSV：

**示例**：
```
Excel 中：
Name{这是英雄的名称，(注释内容)}

CSV 中：
Name
```

**支持的注释位置**：
- 列名中的注释：`TestData2{这里的字符都不会被录入CSV}`
- 单元格中的注释：支持在任意数据单元格中添加注释
- 多层嵌套：`{外层注释{内层注释}}`

#### 1.4 数据格式
- 所有数据以字符串形式存储
- 支持中文、英文、数字等各种字符
- 空单元格会被转换为空字符串

---

### 2. 文件命名约定

#### 2.1 Excel 文件
- **格式**：必须是 .xlsx 格式
- **命名**：建议使用英文，如 Hero.xlsx、Item.xlsx
- **存放位置**：`Assets/Excel2CSV/Excel/`

#### 2.2 CSV 文件（自动生成）
- **命名规则**：与 Excel 文件同名，扩展名为 .csv
- **示例**：Hero.xlsx → Hero.csv
- **存放位置**：`Assets/Excel2CSV/Resources/CSV/`

#### 2.3 C# 脚本文件（自动生成）
- **命名规则**：Excel 文件名 + "CSV"
- **示例**：Hero.xlsx → HeroCSV.cs
- **存放位置**：`Assets/Excel2CSV/ScriptsCS/`

---

## 使用工作流程

### 步骤 1：准备 Excel 数据表

1. 在 `Assets/Excel2CSV/Excel/` 目录下创建或编辑 .xlsx 文件
2. 确保第一行是列名
3. 确保第一列是 ID 列
4. 可以使用 `{}` 添加注释和说明

**示例数据表结构**：
```
| ID | Name    | SKILL | TestData |
|----|---------|-------|----------|
| 1  | Alice   | 111   | 0        |
| 2  | Bob     | 222   | 1        |
| 3  | Charlie | 333   | 2        |
```

### 步骤 2：转换 Excel 到 CSV

1. 在 Unity 编辑器中，点击菜单：`KT CSV Tools > Convert Excel to CSV`
2. 等待转换完成，查看控制台日志
3. 系统会自动：
   - 读取所有 .xlsx 文件
   - 生成对应的 .csv 文件
   - 生成对应的 C# 脚本
   - 刷新 Unity 资源数据库

### 步骤 3：在代码中使用数据

有两种方式读取数据：

#### 方式 1：使用 CSVReader（推荐）
```csharp
using UnityEngine;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        // 读取 Hero 表中 ID 为 "1" 的英雄名字
        string heroName = (string)CSVReader.ReadDataRow("HeroCSV", "1", "Name");
        Debug.Log("英雄名字：" + heroName);
        
        // 读取 Item 表中 ID 为 "1" 的中文名称
        string itemNameCN = (string)CSVReader.ReadDataRow("ItemCSV", "1", "CN");
        Debug.Log("物品名称：" + itemNameCN);
    }
}
```

#### 方式 2：直接使用生成的类
```csharp
using CSV_SPACE;
using UnityEngine;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        // 直接调用生成的加载方法
        HeroCSV hero = HeroCSVLoad.Load("1");
        Debug.Log("英雄名字：" + hero.Name);
        Debug.Log("英雄技能：" + hero.SKILL);
    }
}
```

---

## 示例数据表分析

仓库中包含三个示例数据表：

### 1. Hero.xlsx - 英雄数据表

**CSV 输出**（Hero.csv）：
```csv
ID,Name,SKILL,TestData,TestData2
测试,测试,测试,测试,测试
1,Alice,111,0,测试
2,Bob,222,1,测试
3,Charlie,333,2,测试
...
```

**用途**：存储游戏中的英雄角色数据
**字段**：
- ID：英雄唯一标识
- Name：英雄名称
- SKILL：技能ID或描述
- TestData：测试数据字段
- TestData2：测试数据字段2

### 2. Item.xlsx - 物品数据表

**CSV 输出**（Item.csv）：
```csv
ID,CN,EN,Effect
1,神秘道具1,Item1,1
2,神秘道具2,Item2,2
3,神秘道具3,Item3,3
...
```

**用途**：存储游戏中的物品数据，支持多语言
**字段**：
- ID：物品唯一标识
- CN：中文名称
- EN：英文名称
- Effect：效果ID或描述

### 3. Lan.xlsx - 多语言数据表

**CSV 输出**（Lan.csv）：
```csv
ID,CN,EN
1,王,Wang
2,火,Huon
...
```

**用途**：存储多语言文本对照表
**字段**：
- ID：文本唯一标识
- CN：中文文本
- EN：英文文本

**多语言支持说明**：
通过这种方式，可以轻松实现游戏的国际化：
1. 在 Excel 中维护多语言对照表
2. 根据用户选择的语言，动态读取对应字段
3. 支持扩展更多语言（添加新列即可）

---

## 技术架构分析

### 1. 设计模式

#### 1.1 单例模式
每个 XXXCSVLoad 类都维护一个静态实例：
```csharp
public static HeroCSV herocsv = new HeroCSV();
```

#### 1.2 工厂模式
CSVReader 作为工厂类，通过反射动态创建和加载数据对象。

#### 1.3 编辑器扩展模式
使用 Unity 的 EditorWindow 和 MenuItem 属性扩展编辑器功能。

### 2. 关键技术点

#### 2.1 反射（Reflection）
```csharp
Type typeCSV = Type.GetType(typeName);
Type typeCSVLoad = Type.GetType(typeName + "Load");
object obj = typeCSVLoad.GetMethod("Load").Invoke(null, new object[] { id });
```
**优势**：
- 动态类型访问
- 通用的数据读取接口
- 灵活的扩展性

#### 2.2 Unity Resources 系统
```csharp
var csvTextAsset = Resources.Load<TextAsset>(filePath);
```
**特点**：
- CSV 文件必须放在 Resources 文件夹
- 支持运行时动态加载
- 自动管理资源的加载和卸载

#### 2.3 字符串处理和正则表达式
```csharp
// 移除 {} 标记的内容
columnName = columnName.Remove(startIndex, endIndex - startIndex + 1);
// 移除换行符
columnName = Regex.Replace(columnName, @"[\r\n]+", "");
```

#### 2.4 编辑器资源刷新
```csharp
AssetDatabase.Refresh();
```
**作用**：通知 Unity 重新扫描和导入资源文件

### 3. 数据流程图

```
Excel 文件 (.xlsx)
    ↓
ExcelDataReader 解析
    ↓
数据清洗（移除注释）
    ↓
CSV 文件生成 (.csv)
    ↓
C# 代码生成 (.cs)
    ↓
Unity 编译
    ↓
运行时数据读取
    ↓
游戏逻辑使用
```

---

## 优势与特色

### 1. 开发效率提升
- **无需手写数据类**：自动生成所有数据访问代码
- **快速迭代**：修改 Excel 后一键重新生成
- **减少错误**：避免手动输入数据导致的错误

### 2. 设计师友好
- **Excel 编辑**：使用熟悉的 Excel 工具编辑数据
- **可视化管理**：直观的表格界面
- **支持注释**：可以在数据中添加说明和文档

### 3. 多语言支持
- **简单的多语言方案**：通过不同列存储不同语言
- **易于扩展**：添加新语言只需添加新列
- **统一管理**：所有语言文本集中在一个表格中

### 4. 灵活的数据访问
- **静态方法**：可以直接调用 XXXCSVLoad.Load()
- **反射访问**：通过 CSVReader 实现通用访问
- **类型安全**：生成的类提供属性访问

### 5. 轻量级设计
- **无外部依赖**：除了 ExcelDataReader 库外无其他依赖
- **简单实现**：代码结构清晰，易于理解和修改
- **性能良好**：CSV 解析速度快，适合小到中型数据量

---

## 潜在的改进方向

### 1. 类型系统增强
**当前状况**：所有字段都是字符串类型
**改进建议**：
- 支持在 Excel 中指定数据类型（int、float、bool 等）
- 自动生成对应类型的属性
- 提供类型转换和验证

### 2. 数据验证
**改进建议**：
- 添加数据完整性检查
- 验证 ID 的唯一性
- 检查必填字段
- 数据格式验证

### 3. 性能优化
**改进建议**：
- 使用字典（Dictionary）缓存数据，避免每次都解析 CSV
- 实现数据预加载机制
- 支持批量数据读取

### 4. 错误处理增强
**改进建议**：
- 更详细的错误信息
- 数据加载失败时的降级策略
- 日志系统优化

### 5. 编辑器功能扩展
**改进建议**：
- 添加 Excel 预览功能
- 提供数据对比工具
- 支持选择性转换（只转换修改过的文件）
- 添加进度条显示

### 6. 序列化支持
**改进建议**：
- 支持导出为 JSON、XML 等其他格式
- 支持二进制序列化以提高性能
- 支持数据加密

### 7. 数组和对象支持
**改进建议**：
- 支持数组类型字段（如技能列表）
- 支持嵌套对象
- 支持引用其他表的数据

---

## 适用场景

### 1. 适合使用的场景
- ✅ 中小型游戏项目的数据配置
- ✅ 静态数据表（装备、技能、关卡等）
- ✅ 多语言文本管理
- ✅ 游戏数值配置（属性、成长曲线等）
- ✅ 需要策划频繁修改的数据

### 2. 不太适合的场景
- ❌ 大规模数据（建议使用数据库）
- ❌ 需要复杂查询的场景
- ❌ 需要实时更新的动态数据
- ❌ 需要高性能批量读取的场景

---

## 总结

Unity-Excel2CSV-And-DataReaderTool 是一个设计精巧、实用性强的 Unity 数据管理工具。它通过自动化的方式，将 Excel 表格转换为游戏可用的数据格式，并生成类型安全的访问代码，大大提高了游戏开发的效率。

**核心价值**：
1. **降低技术门槛**：设计师和策划可以直接使用 Excel 管理数据
2. **提高开发效率**：自动化代码生成，减少重复劳动
3. **减少错误率**：避免手动数据输入错误
4. **便于维护**：统一的数据管理方式，易于查找和修改

**技术亮点**：
1. 使用编辑器扩展提供友好的工具界面
2. 基于反射实现灵活的数据访问
3. 智能的注释过滤机制
4. 自动化的代码生成流程

这个工具特别适合中小型 Unity 项目，能够显著提升团队的数据配置效率，是一个值得推广使用的实用工具。
