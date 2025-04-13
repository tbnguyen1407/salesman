{{- max_width = 40 }}
{{- company_name = "DragonAsia" }}
{{- company_address = "Spiegelstr.3 68305 Mannheim-Luzenberg" }}
{{- company_phone = "+49 1573 0653396" }}
{{- taxref_number = "37303/32421" }}

{{- company_name | string.truncate max_width "" }}
{{ company_address | string.truncate max_width "" }}
{{ company_phone | string.truncate max_width "" }}

Invoice: {{ id | string.upcase | string.truncate max_width "" }}
Taxref : {{ taxref_number }}
Date   : {{ timestamp | date.to_string '%F %T'}}

--- Customer ---
{{ customer.name ?? "<no_name>" | string.truncate max_width "" }}
{{ customer.address ?? "<no_address>" | string.truncate max_width "" }}
{{ customer.phone ?? "<no_phone>" | string.truncate max_width "" }}

--- Items ---
{{ "Qty" | string.pad_right 4 }} {{ "Name" | string.pad_right 28 }} {{ "Price" | string.pad_left 7 }}
{{ for item in cart_items }}
{{- item.quantity | string.pad_right 4 }} {{ item.code + " " + item.name | string.pad_right 28 | string.truncate 28 "" }} {{ item.price * item.quantity | string.pad_left 7 }}
{{ end -}}
---

{{ "Total SGD (8% GST included):" | string.pad_right (max_width - 10) }} {{ total_price | string.pad_left 10 -}}
