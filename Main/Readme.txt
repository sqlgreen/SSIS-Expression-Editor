*** Projects ***

There a three solution files, one each for SQL Server 2005, SQL Server 2008 including R2 and SQL Server 2012.

SQL Server 2005 versions of the project files all contain the compilation symbol YUKON.
SQL Server 2008 versions of the project files all contain the compilation symbol KATMAI.
SQL Server 2012 versions of the project files all contain the compilation symbol DENALI.
Conditional compilation if blocks in the code reference 2005 vs 2008 vs 2012 interface names or specific code.
Project files are specific to the SQL Server version but the code files are written for all.


** Assembly Versions **
Version 1 - SQL Server 2005
Version 2 - SQL Server 2008
Version 3 - SQL Server 2012
 

** ExpressionEditorNNNN ** 
The expression editor conttrol library. 
ExpressionEditorPublic is a reusable form for custom tasks.
ExpressionEditorView is the editor control if you wish to host it yourself.


** ExpressionTesterNNNN **
The standalone tool.


** PackageTestNNNN **
A simple test harness used to test the ExpressionEditorPublic form.



Darren Green
October 2012
http://www.konesans.com