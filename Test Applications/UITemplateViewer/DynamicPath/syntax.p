->
{mixed_path}|{local_doc_path}|{std_xpath}|{literal_statement}
$

root->
^{element_group}+$
$

filter->
{equal_filter}|{nonequal_filter}
$

elementGroup->
^{element_group_capture}+$
$

node->
{attribute}|{element}|{self}|{parent}|{literal}|{wildcard}|{element_lookup}
$

std_xpath_capture->
{element_group}+
$

local_doc_path_capture->
{element_group}+
$

{mixed_path}->
^\[(?<local_doc_path_capture>.+)\]\{(?<std_xpath_capture>.+)\}$
$

{local_doc_path}->
^\[(?<local_doc_path_capture>.+)\]$
$

{std_xpath}->
^\{(?<std_xpath_capture>.+)\}$
$

{literal_statement}->
(?<literal_statement>^[^\[\{].*$)
$

{zero_to_two_slashes}->
/{0,2}
$

{non_bracket}->
[^\[\]]
$

{non_bracket_non_slash}->
[^\[\]/]
$

{bracket_scope}->
\[(?<d>)|\](?<-d>)
$

{scope_check}->
(?(d)(?!))
$

{bracket_group}->
(\[(?>{non_bracket}+|{bracket_scope})*{scope_check}\])?
$

{bracket_group_capture}->
(\[(?<filter>(?>{non_bracket}+|{bracket_scope})*{scope_check})\])?
$

{non_bracket_non_equal}->
[^\[\]=]
$

{filter_part}->
{non_bracket_non_equal}+{bracket_group}
$

{equal_filter}->
(?<root>^{filter_part})(?<sign>=)(?<root>{filter_part}$)
$

{nonequal_filter}->
(?<root>^{filter_part}$)
$

{non_bracket_non_slash_capture}->
(?<node>{non_bracket_non_slash}*)
$

{zero_to_two_slashes_capture}->
(?<hierarchy>{zero_to_two_slashes})
$

{element_group}->
(?<elementGroup>{zero_to_two_slashes}{non_bracket_non_slash}*{bracket_group})
$

{element_group_capture}->
({zero_to_two_slashes_capture}{non_bracket_non_slash_capture}{bracket_group_capture})
$

{wildcard}->
(?<wildcard>^\*$)
$

{element}->
(?<element_capture>({identifier}::)?{identifier})
$

{element_lookup}->
(?<element_lookup>:{identifier})
$

element_capture->
((?<namespace>{identifier})::)?(?<element_name>{identifier})
$

{attribute}->
(?<attribute>@{identifier})
$

{identifier}->
[a-zA-Z]+
$

{self}->
(?<self>^\.$)
$

{parent}->
(?<parent>\.\.)
$

{literal}->
(?<literal>'[^']*')
$