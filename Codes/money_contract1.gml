var prepaid = ds_map_find_value(_contract_map, "Prepaid")
if (prepaid <= 0)
    prepaid = 0
var money = (ds_map_find_value(_contract_map, "Gold") - prepaid)
money *= global.contract_money_modifier