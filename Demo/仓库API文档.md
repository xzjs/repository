# 仓库API文档
说明：

* 接口需要支持跨域，否则网页端无法获取到数据
* 货物汇总接口无需开发，获取货物后程序会自动计算
* 


## 获取货物
method：GET

url：？

return：

```
[
    {
        "id": 1,
        "no": "1",
        "type": "0",
        "created_at": null,
        "updated_at": null,
        "floors": [
            {
                "id": 1,
                "no": "1",
                "shelf_id": "1",
                "created_at": null,
                "updated_at": null,
                "cells": [
                    {
                        "id": 1,
                        "no": "1",
                        "floor_id": "1",
                        "created_at": null,
                        "updated_at": null,
                        "good": {
                            "id": 1,
                            "name": "棉衣裤",
                            "model_name": "mianyiku",
                            "unit": "条",
                            "num": "5",
                            "cell_id": "1",
                            "created_at": null,
                            "updated_at": null
                        }
                    }
                ]
            }
        ]
    }
]
```
##获取配置
method：GET

url：？

return：

```
{
    "moving_speed(移动速度)": 5,
    "spinning_speed(转向速度)": 5,
    "zoom_speed(缩放速度)": 20,
    "selected_color(选中颜色)": {
        "r": 0,
        "g": 0,
        "b": 0,
        "a": 0
    },
    "window_color(窗口颜色)": {
        "r": 0,
        "g": 0,
        "b": 0,
        "a": 0
    }
}
```
## 货物流动
method：GET

url：？

parameter：棉被（货物名称，string类型）

return：

```
[
    {
        "name": "棉衣裤",
        "action": "入库",
        "time": "2017-05-12 10:06:09",
        "num": 600,
        "unit": "条"
    }
]
```
## 调拨记录
method:GET

url:?

return:

```
[
    "调拨记录",
    "调拨记录",
    "调拨记录"
]
```

