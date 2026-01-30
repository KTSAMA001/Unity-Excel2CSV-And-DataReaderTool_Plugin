# Unity-Excel2CSV-And-DataReaderTool Repository Analysis

## Executive Summary

Unity-Excel2CSV-And-DataReaderTool is a practical Unity plugin that automates game data management by converting Excel spreadsheets (.xlsx) to CSV files and automatically generating C# accessor classes. This tool significantly improves development efficiency by eliminating manual data entry code and enabling rapid iteration on game data.

## Core Features

### 1. Excel to CSV Converter
**File**: `Assets/Excel2CSV/ExcelToCSVConverterEditor.cs`

- Unity Editor menu integration: "KT CSV Tools"
- Automatic conversion of .xlsx files to .csv format
- Smart comment filtering using `{}` brackets
- Automatic C# code generation for data access

### 2. CSV Data Reader
**File**: `Assets/Excel2CSV/CSVReader.cs`

- Static utility class for runtime data access
- Uses reflection for dynamic type and field access
- Provides generic `ReadDataRow()` method
- Supports querying by ID and field name

### 3. Auto-Generated Data Classes
**Location**: `Assets/Excel2CSV/ScriptsCS/`

For each Excel file, generates:
- Data class (e.g., `HeroCSV`) - inherits from `CSVBase`
- Loader class (e.g., `HeroCSVLoad`) - provides static `Load()` method

### 4. Third-Party Library: ExcelDataReader
**Location**: `Assets/Excel2CSV/Plugins/ExcelDataReader-develop/`

- Lightweight C# Excel file reader
- Supports .xlsx, .xlsb, .xls formats
- Handles Excel versions from 2.0 to 2019+

## Directory Structure

```
Assets/Excel2CSV/
├── Excel/                    # Source Excel files (input)
│   ├── Hero.xlsx
│   ├── Item.xlsx
│   └── Lan.xlsx
│
├── Resources/CSV/           # Generated CSV files (intermediate)
│   ├── Hero.csv
│   ├── Item.csv
│   └── Lan.csv
│
├── ScriptsCS/               # Generated C# scripts (output)
│   ├── HeroCSV.cs
│   ├── ItemCSV.cs
│   └── LanCSV.cs
│
├── Plugins/                 # Third-party plugins
│   └── ExcelDataReader-develop/
│
├── CSVBase.cs               # Base class for CSV data classes
├── CSVReader.cs             # CSV data reader utility
├── ExcelToCSVConverterEditor.cs   # Editor converter tool
└── Test.cs                  # Sample usage script
```

## Data Table Rules

### Excel Structure Requirements

1. **First row must be column headers** - Used as C# property names
2. **First column must be ID** - Used as primary key for data lookup
3. **Use `{}` for comments** - Content in braces is filtered out from CSV
4. **All data stored as strings** - Automatic conversion handled by generated classes

### Naming Conventions

- **Excel files**: Must be .xlsx format (e.g., Hero.xlsx)
- **CSV files**: Auto-generated with same name (e.g., Hero.csv)
- **C# scripts**: Excel name + "CSV" suffix (e.g., HeroCSV.cs)

## Usage Workflow

### Step 1: Prepare Excel Data
Create/edit .xlsx files in `Assets/Excel2CSV/Excel/` with proper structure:
- Row 1: Column names (headers)
- Column 1: ID values (unique identifiers)
- Use `{}` for notes that shouldn't appear in CSV

### Step 2: Convert to CSV
1. In Unity Editor: `KT CSV Tools > Convert Excel to CSV`
2. System automatically:
   - Reads all .xlsx files
   - Generates .csv files
   - Generates C# accessor classes
   - Refreshes Unity asset database

### Step 3: Use in Code

**Method 1: Using CSVReader (Recommended)**
```csharp
using UnityEngine;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        // Read hero name with ID "1"
        string heroName = (string)CSVReader.ReadDataRow("HeroCSV", "1", "Name");
        Debug.Log("Hero Name: " + heroName);
    }
}
```

**Method 2: Direct Class Usage**
```csharp
using CSV_SPACE;
using UnityEngine;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        HeroCSV hero = HeroCSVLoad.Load("1");
        Debug.Log("Hero Name: " + hero.Name);
        Debug.Log("Hero Skill: " + hero.SKILL);
    }
}
```

## Example Data Tables

### Hero.xlsx - Character Data
```csv
ID,Name,SKILL,TestData,TestData2
1,Alice,111,0,Test
2,Bob,222,1,Test
3,Charlie,333,2,Test
```
**Purpose**: Store game character/hero data

### Item.xlsx - Item Data with Localization
```csv
ID,CN,EN,Effect
1,神秘道具1,Item1,1
2,神秘道具2,Item2,2
```
**Purpose**: Store item data with multi-language support

### Lan.xlsx - Localization Table
```csv
ID,CN,EN
1,王,Wang
2,火,Fire
```
**Purpose**: Maintain language translation mappings

## Technical Architecture

### Design Patterns

1. **Singleton Pattern**: Each loader class maintains a static instance
2. **Factory Pattern**: CSVReader acts as factory using reflection
3. **Editor Extension Pattern**: Uses Unity's EditorWindow and MenuItem

### Key Technologies

1. **Reflection**: Dynamic type and method invocation
2. **Unity Resources System**: Runtime asset loading
3. **Regular Expressions**: String processing and comment filtering
4. **Asset Database**: Editor resource management

### Data Flow

```
Excel Files (.xlsx)
    ↓
ExcelDataReader Parsing
    ↓
Data Cleaning (Remove comments)
    ↓
CSV Generation (.csv)
    ↓
C# Code Generation (.cs)
    ↓
Unity Compilation
    ↓
Runtime Data Access
    ↓
Game Logic
```

## Advantages

### 1. Development Efficiency
- No need to write data access classes manually
- One-click regeneration after Excel modifications
- Reduces human errors in data entry

### 2. Designer-Friendly
- Familiar Excel interface for data editing
- Visual table management
- Support for inline documentation via comments

### 3. Localization Support
- Simple multi-language solution using columns
- Easy to extend with new languages
- Centralized text management

### 4. Flexible Data Access
- Static methods for direct access
- Reflection-based generic access via CSVReader
- Type-safe property access in generated classes

### 5. Lightweight Design
- Minimal external dependencies
- Clear code structure
- Good performance for small to medium datasets

## Potential Improvements

### 1. Type System Enhancement
- Support for specifying data types in Excel (int, float, bool, etc.)
- Generate strongly-typed properties
- Type conversion and validation

### 2. Data Validation
- Integrity checks
- ID uniqueness validation
- Required field validation
- Format validation

### 3. Performance Optimization
- Dictionary caching to avoid repeated CSV parsing
- Data preloading mechanism
- Batch data loading support

### 4. Enhanced Error Handling
- More detailed error messages
- Fallback strategies for failed loads
- Improved logging system

### 5. Editor Features
- Excel preview functionality
- Data comparison tools
- Selective conversion (only modified files)
- Progress bar display

### 6. Serialization Support
- Export to JSON, XML formats
- Binary serialization for performance
- Data encryption support

### 7. Complex Data Types
- Array/list field support
- Nested object support
- Cross-table data references

## Use Cases

### ✅ Suitable For:
- Small to medium-sized game projects
- Static data tables (equipment, skills, levels, etc.)
- Multi-language text management
- Game configuration values
- Data requiring frequent designer updates

### ❌ Less Suitable For:
- Large-scale datasets (consider databases)
- Complex query requirements
- Real-time dynamic data
- High-performance batch reading scenarios

## Conclusion

Unity-Excel2CSV-And-DataReaderTool is a well-designed, practical tool for Unity game data management. Through automation, it converts Excel spreadsheets into game-ready data formats with type-safe accessor code, significantly improving game development efficiency.

**Core Value:**
1. Lowers technical barriers for designers and planners
2. Improves development efficiency through automation
3. Reduces error rates from manual data entry
4. Easy to maintain with unified data management

**Technical Highlights:**
1. User-friendly editor extension interface
2. Flexible data access via reflection
3. Intelligent comment filtering mechanism
4. Automated code generation pipeline

This tool is particularly suitable for small to medium Unity projects and can significantly boost team productivity in data configuration tasks.
