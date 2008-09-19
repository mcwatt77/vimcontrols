namespace VIMControls
{
    public class Parser
    {
    }
    /*
[0,.] => 0
maybe xpath/path_element/self_path_element
maybe xpath/path_element/parent_path_element
state 1
[0,/] => 0
maybe xpath/xpath_node/path_search_token/child_search_token
maybe xpath/xpath_node/path_search_token/descendant_search_token
state 3
[1,.] => 1
not //child_search_token
close //descendant_search_token/..
state 2
[1,] => 1
not //parent_path_element
close //self_path_element/..
state 2
[2,/] => 1,2
maybe xpath_node/path_search_token/child_search_token
maybe xpath_node/path_search_token/descendant_search_token
state 3
[3,/] => 3
not //child_search_token
close //descendant_search_token/..
state 4
[3,] => 3
not //descendant_search_token
close //child_search_token/..
state 4
[4,.] => 4
maybe path_element
state 1
[4,a] => 4
search 'ncestor([^a-zA-Z_-0-9])' '$1'
close node/element_search/special_element_search/element_search_type
state 2
[4,c] => 4
search 'hild([^a-zA-Z_-0-9])' '$1'
close node/element_search/special_element_search/element_search_type
state 2
[4,d] => 4
search 'escendant([^a-zA-Z_-0-9])' '$1'
close node/element_search/special_element_search/element_search_type
state 2
[4,[a-zA-Z_]] => 4
close node/element_search/element_id/id/first_id_letter
state 5
      
      
xpath -> node.separated_by(path_search_token)
separated_by -> this (arg0 this)*
node -> element_search[criteria]
path_search-token -> child_search_token | descendant_search_token
child_search_token -> /
descendant_search_token -> //
first_id_letter -> [a-zA-Z_]
id_letter -> first_id_letter | [0-9-]
id -> first_id_letter id_letter*
element_search -> element_id | special_element_search
element_id -> id
special_element_search -> element_search_type special_element_search_clause?
element_search_type -> 'ancestor' | 'descendant' | 'child'
special_element_search_clause -> '::' special_element_search_element_name
special_element_search_element_name -> element_id
criteria -> '[' wsp* clause.separated_by(wsp? logic_operator wsp?) wsp* ']'
wsp -> ' '+
logic_operator -> 'and' | 'or'
clause -> parameter_clause | comparison_clause
parameter_clause -> grouped_clause | function_clause | literal_clause | element_clause
grouped_clause -> '(' clause ')'
function_clause -> function_name '(' parameters ')'
function_name -> id
parameters -> criteria.separated_by(wsp* parameter_clause wsp*)
comparison_clause -> parameter_clause comparison_operator parameter_clause
comparison_operator -> '=' | '!=' | '>' | '<' | '<=' | '>='
literal_clause -> '\'' [^\']* '\''
element_clause -> '@'? id
     * 
     * */
}
