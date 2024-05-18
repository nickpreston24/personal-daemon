drop procedure if exists SearchScriptures;
DELIMITER ^_^
CREATE PROCEDURE SearchScriptures(
    term varchar(250)
    #     ,is_archived bit,
#     is_deleted bit,
#     is_enabled bit
)
BEGIN
    #     compute temp vars first
    #     set @search_term = coalesce(search_term, '%');

    set @search_term = IF(TRIM(COALESCE(term, '')) = '', '%', term);

    #     set @regex_symbols = '[[\{\}\.\s\d\w]\+?\*?]+';
    #     set @regex_symbols = '(\\[sdw]|[.])[+*?]*(\{\d*,?\d*\})?';
    set @regex_symbols = '^(\\[sdw]|[.])[+*?]*$';
    #     set @search_term = 'Hello there! \\s \\d \\w \\.';
    -- https://regex101.com/r/YYkqhs/1
    set @swapped = REGEXP_REPLACE(concat(@search_term, ''), @regex_symbols, '');
    #     set @symbols_found = regexp_substr(search_term, @regex_symbols);
    set @regex_symbol_count = abs(length(@search_term) - length(@swapped));
    set @is_regex_search = @regex_symbol_count > 0;


    # Aggregate anything you wish to search with wildcards (%).
    set @all_text = concat(
            @search_term
        );

    # wildcards can come from user inputs or COALESCES
    set @wildcard_count = abs(LENGTH(@all_text) - LENGTH(REPLACE(@all_text, '%', '')));

    # Disabwildcard_countle full text search if search term is empty
    set @use_full_text_search = IF(TRIM(COALESCE(@search_term, '')) = '', 0, 1);
    #     select @use_full_text_search as "using full text search?";
    # Toggle exact match search
    -- if full-text search is on, we don't want to match exact.
    set @match_exact =
            case
                when
                    (@wildcard_count = 0 AND @use_full_text_search = 0 and @is_regex_search = 0) then 1
                else
                    @wildcard_count = 0
                end;

    -- debug
    select
        -- @symbols_found,
        @use_full_text_search as "using full text search?",
        @regex_symbol_count   as 'number of regex symbols',
        @search_term          as 'your search',
        @swapped,
        @match_exact,
        @is_regex_search;
    #debug

    #     set @debug_mode = 1;
#     case when @debug_mode = 1  then (
#     select @search_term
#          , @all_text
#          , @is_regex_search
#          , @regex_symbol_count
#          , @swapped
#          , @wildcard_count
#          , @match_exact
#     ) else 0 end;

    SELECT scripturerow.created_at
         , scripturerow.modified_at
         , scripturerow.text
         , scripturerow.Name
#          , scripturerow.is_archived
#          , scripturerow.is_enabled
#          , scripturerow.is_deleted
         #          , MATCH(scripturerow.created_by, scripturerow.modified_by, scripturerow.text)
         # #                  AGAINST(@search_term IN NATURAL LANGUAGE MODE) AS score
         #                 AGAINST(@search_term IN BOOLEAN MODE) AS score
#          , @match_exact
#          , @use_full_text_search
#          , @is_regex_search
#          , @wildcard_count
#          , @search_term
    FROM scriptures scripturerow
    where 1 = 1
        # text filters
        AND (
                          @match_exact = 1 AND (
                              scripturerow.created_by = @search_term
                          OR scripturerow.modified_by = @search_term
                          OR scripturerow.Text = @search_term
                          OR scripturerow.Name = @search_term
                      )
                  XOR
                          @match_exact = 0 AND (
                                      scripturerow.created_by like @search_term
                                  OR scripturerow.modified_by like @search_term
                                  OR scripturerow.text like @search_term
                                  OR scripturerow.Name like @search_term
                              )
              )
       OR @is_regex_search = 1 AND (
                scripturerow.created_by regexp @search_term
            OR scripturerow.modified_by regexp @search_term
            OR scripturerow.text regexp @search_term
            OR scripturerow.Name regexp @search_term
        )
        # flags
#        OR (
#                       is_archived is not null and scripturerow.is_archived = is_archived
#                   xor
#                       is_deleted is not null and
#                       scripturerow.is_deleted = is_deleted
#                   xor
#                       is_enabled is not null and
#                       scripturerow.is_enabled = is_enabled
#               )
        xor
          (@use_full_text_search = 1 and MATCH(scripturerow.created_by, scripturerow.modified_by, scripturerow.text)
                                               AGAINST(@search_term IN NATURAL LANGUAGE MODE))

    #     order by score desc
    ;
END ^_^

DELIMITER ;
DELIMITER ;
# 
# 
# select *
# from scriptures
# order by modified_at desc;

call SearchScriptures('%sample%');
# call SearchScriptures('\d+'); # need work

select id,
       name,
       text,
       modified_at
from scriptures
order by modified_at desc;

select distinct name,
                translation_abbreviation,
                translation,
                source_url,
                text
from scriptures;

# Count scriptures
select count(name) total_scriptures
from scriptures;
# delete from scriptures where id >=1;

# 
# # logs
select exception_message, exception_text, application_name, created_at, modified_at
from logs
where exception_message is not null
  and exception_message != ''
order by exception_message desc,
         modified_at desc
;
# 
# update logs
# set created_at = now(), modified_at = now()
# where application_name = 'personal_daemon';
# 

call SearchLogs('railway', null, null, null);

# prune empty texts 
/*
delete
from scriptures
where text is null
   or text = ''
   or text = '@text'
   or name is null
   or name = ''
   or name = '@name'


delete
from scriptures
where chapter is null or chapter = ''

select distinct name,
                chapter,
                text
from scriptures;
 */

