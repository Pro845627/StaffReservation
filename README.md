# English - Staff Reservation
  Allows specified players like staff can join the server while the server is full. In the meantime, the displayed `maxplayers` excludes the reservation slots.
  The plugin is recommended for official servers whose max players is limited.
  
  - Disclaimer: This plugin does not violate VSR (yep I checked it).
  - Be careful: The `group_list` config is **ONLY SUPPORTS SHARED GROUPS**.
  
  ```yaml
  staff_reservation:
    #Indicates whether the plugin is enabled or not
    is_enabled: true
    #The max slots of reservation.
    max_slots: 1
    #List of groups who will be in reservation.
    groups_list:
      - owner
      - admin
      - moderator
    #List of players who will be in reservation. If the sepcified player is a member of any group above, you don't need to add him.
    userid_list:
      - SomeOnesUserId@steam
  ```
  - Gameplay Configs:
    - `max_players` : Set it to normal slots + reservation slots（eg. normal 25, reservation 2, `max_players` set to 27）
  
  # Simplified Chinese - 管理预留位
  在服务器满人时，允许将像管理人员或指定的玩家进入服务器。同时，显示的`最大玩家数`将不会包含预留位数。
  本插件推荐使用于有最大人数限制的官方服务器。
  - 免责声明：本插件不会违反VSR（没戳我检查过了）
  - 注意：`group_list`配置项**仅对分享的组有效**
  
  ```yaml
  staff_reservation:
    #是否启用本插件
    is_enabled: true
    #最大预留位数
    max_slots: 1
    #支持预留位的组
    groups_list:
      - owner
      - admin
      - moderator
    #支持预留位的玩家。如果指定玩家是上方任何一个组的成员, 则不需要再添加该玩家。
    userid_list:
      - 某个人的ID@steam
  ```
  
   - Gameplay配置:
   - `max_players` : 设置为普通位 + 预留位（例：普通 25, 预留 2, `max_players` 设为 27）
