var list_artifacts = __dsDebuggerListCreate()
for (var i = 0; i < 5; i += 1)
{{
    if (ds_list_find_index(scr_atr("specialItemsPool"), object_get_name(artifacts[i])) == -1)
        ds_list_add(list_artifacts, artifacts[i])
}}
if (ds_list_size(list_artifacts) > 0)
{{
    ds_list_shuffle(list_artifacts)
    if scr_chance_value(50)
        scr_inventory_add_item(ds_list_find_value(list_artifacts, 0), id, -4, 1, -4, 1, 1)
    else
    {{
        {0} 
        {{
            scr_inv_atr_set("Duration", irandom_range(35, 60))
        }}
    }}
}}
else
{{
    {0} 
    {{
        scr_inv_atr_set("Duration", irandom_range(35, 60))
    }}
}}
__dsDebuggerListDestroy(list_artifacts)