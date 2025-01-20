# ET加Android Studio自动构建脚本

## 前置环境
Python3.*、powershell本地脚本运行权限、Windows环境、Unity

## 使用方法
1、将资源目录下**LocalBuildTools.cs**文件放入项目**Assets**目录下任一**Editor**文件夹下
2、配置**Gradle环境变量**，在**Android Studio**里配置**gradle路径**并执行**make project**, unity导出一次后，在导出目录下执行```gradle wrapper```
3、填写**env.ini**文件内环境变量
- work_space ：自动构建脚本路径
- unity ：unity.exe路径
- path ：unity项目路径
- log ：输出日志位置
- output_path ：导出路径
- release_work_path : 导出工程文件目录
- apk_path : apk输出路径
- aab_path : aab输出路径
- file ：签名文件路径
- password ：签名文件密码
- alias ：签名文件字符集
- key_password ：签名文件字符集密码
4、使用
``` bat
python <env.work_space>\Build.py -c <代码版本> -d <包体类型> -r <是否重命名最终出包> -p <资源分发类型> -o <覆盖安装提示> -f <是否强制清理缓存> -t <构建模板名称>
```

## 执行流程
1.检查配置unity是否有运行实例，有则关闭
2.以批处理模式开启unity构建并输出日志(https://docs.unity.cn/cn/2021.1/Manual/CommandLineArguments.html)
3.使用gradlew开启android studio构建(https://developer.android.google.cn/build/building-cmdline?hl=zh-cn)
4.检查是否需要重命名并进行操作
5.清理临时文件

## 最终输出位置
apk : <env.output_path>\launcher\build\outputs\apk\<debug or release>\*.apk
aab : <env.output_path>\launcher\build\outputs\bundle\<debug or release>\*.aab