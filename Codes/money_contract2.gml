var money_limit = 0;
if (instance_exists(o_inv_moneybag))
{
    money_limit = o_inv_moneybag.stack_limit;
}
else
{
    var tmp_moneybag = instance_create_depth(-15000, -15000, 0, o_inv_moneybag);
    money_limit = tmp_moneybag.stack_limit;
    instance_destroy(tmp_moneybag);
}
with (scr_guiCreateContainer(global.guiBaseContainerVisible, o_reward_container))
{
    while (money > money_limit)
    {
        with (scr_inventory_add_item(o_inv_moneybag))
        {
            ds_map_replace(data, "Stack", math_round(money_limit))
        }
        money -= money_limit
    }
    with (scr_inventory_add_item(o_inv_moneybag))
    {
        ds_map_replace(data, "Stack", math_round(money))
    }
}