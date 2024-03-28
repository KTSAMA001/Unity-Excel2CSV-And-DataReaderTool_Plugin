# Unity-Excel2CSV-And-DataReaderTool
通过Excel表格存储数据，转换到CSV，然后可在Unity中直接读取使用，更快捷的数据修改，直接修改Excel然后重新生成对应文件即可

Excel（xlsl格式表格）To CSV，CSV读取数据工具
文件结构
 ![image](https://github.com/KTSAMA001/Unity-Excel2CSV-And-DataReaderTool_Plugin/assets/120698324/fd28278c-97d5-460e-be01-4e5092ff3814)

Assets/Excel2CSV/CSV:生成的CSV文件所在位置
Assets/Excel2CSV/Excel：Excel表格存放位置
Assets/Excel2CSV/Plugins：表格文件IO的Core
Assets/Excel2CSV/ScriptsCS：生成的用于获取CSV数据的cs脚本所在位置
Excel表格上数据：
 ![image](https://github.com/KTSAMA001/Unity-Excel2CSV-And-DataReaderTool_Plugin/assets/120698324/15a6c09b-7b4b-41a6-8b0f-913917cdbf3a)

转换成为的CSV文件：
 ![image](https://github.com/KTSAMA001/Unity-Excel2CSV-And-DataReaderTool_Plugin/assets/120698324/29313d26-df2b-44e4-af6d-e48f7a857090)

规则
{}内部所有数据包括{}将不会被计入表中；

Excel表格第一行为列名称行，将会被计入到脚本之中，是取用数据的字段名称
自动生成的CS脚本名称为csv文件名称+CSV
例如：Hero.xlsl文件会产生 Hero.csv以及HeroCSV.cs文件；
数据备注
Excel表格中在规则之内填写任意的备注或是换行都不影响CSV正常的数据区取用
规则示例：
Excel中：
TestData2{这里的字符都不会被录入CSV{这里的字符都不会被录入CSV}}
Name{这是英雄的名称，
(这里的字符都不会被录入CSV)（这里的字符都不会被录入CSV）}

 CSV中：
 ![image](https://github.com/KTSAMA001/Unity-Excel2CSV-And-DataReaderTool_Plugin/assets/120698324/a6eb84a4-946e-4b1e-9448-ccd7a531a698)



使用方式
第一步：按照提供的示例制作Excel表格；
第二部：![image](https://github.com/KTSAMA001/Unity-Excel2CSV-And-DataReaderTool_Plugin/assets/120698324/0d8df1d1-805c-4e0a-94f8-ab5b7a692ddf)
 生成CSV文件以及cs脚本；

第三步： ![image](https://github.com/KTSAMA001/Unity-Excel2CSV-And-DataReaderTool_Plugin/assets/120698324/f0378c9a-8680-465c-8af5-8b3705de358e)

直接使用，表格第一列的ID将会被作为key用于获取同行的数据；







多语言用法:
提示：![image](https://github.com/KTSAMA001/Unity-Excel2CSV-And-DataReaderTool_Plugin/assets/120698324/233c49ef-61f8-4286-9fb7-47003d1b39be)

 
