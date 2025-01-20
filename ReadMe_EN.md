# ET with Android Studio Automatic Build Script 

## Prerequisites
Python 3.*, local script execution rights for PowerShell, Windows environment, Unity 

## Use Method
1. Place the **LocalBuildTools.cs** file in the **Assets** directory under any **Editor** folder in the resource directory.

2. Configure the **Gradle environment variable**, configure the **gradle path** in **Android Studio** and execute **make project**, Unity exports once, and execute **gradle wrapper** in the export directory.

3. Fill in the environment variables in the **env.ini** file:
- work_space: path of the automatic build script
- unity: path of Unity.exe
- path: Unity project path
- log: output log location
- output_path: export path
- release_work_path : export project path
- apk_path : apk output path
- aab_path : aab output path
- file: signature file path
- password: signature file password
- alias: signature file character set
- key_password: signature file character set password 

4. Use ``` bat
python <env.work_space>\Build.py -c <code_version> -d <package_type> -r <rename_final_package> -p <resource_distribution_type> -o <overwrite_install_prompt> -f <need_force_cache_clearing> -t <constructer_template>
```


The Process:
1. Check if Unity is running an instance, if so, close it.
2. Start Unity build in batch mode and output logs (https://docs.unity.cn/cn/2021.1/Manual/CommandLineArguments.html).
3. Use gradlew to start Android Studio build (https://developer.android.google.cn/build/building-cmdline).
4. Check if renaming is needed and perform the operation.
5. Clear temporary files. 

The final output location apk : <env.output_path>\launcher\build\outputs\apk\<debug or release>\*.apk
aab : <env.output_path>\launcher\build\outputs\bundle\<debug or release>\*.aab