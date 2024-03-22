var money_limit = o_inv_moneybag.stack_limit
with (scr_guiCreateContainer(global.guiBaseContainerVisible, o_reward_container))
{
    with (o_inv_moneybag)
    {
        var money_limit = stack_limit
    }
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