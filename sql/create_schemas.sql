drop table if exists scriptures;
create table scriptures
(
    id                       int          NOT NULL AUTO_INCREMENT, -- not very critical, but nice for tracking how many rows and what we've removed.
    Name                     varchar(250),                         # The name that comes to mind when people quote the bible
    Text                     longtext,                             # actual scripture text
    source_url               varchar(250) null default '',         # e.g. thepathoftruth.com
    translation              varchar(150),                         # the specific translation this comes from
    translation_abbreviation varchar(10),                          # the specific translation this comes from, but abbreviated

    # self/meta
    modified_at              datetime     null default NOW(),
    created_at               datetime     null default NOW(),
    modified_by              varchar(250),
    created_by               varchar(250),

    # PK's
    PRIMARY KEY (id),
    # full-text search
    FULLTEXT (Name, Text)
);

#  I want to keep Names, translations and chapters unique
ALTER TABLE scriptures
    ADD UNIQUE KEY `unique_translations` (`Name`, `translation_abbreviation`);

## Samples
/**

insert into scriptures (name, translation, translation_abbreviation, source_url)
values ('John', 'English Standard Version', 'ESV', 'thepathoftruth.com')
     , ('John', 'New Literal Translation', 'NLT', 'thepathoftruth.com')
#      , ('John', 'English Standard Version', 'NLT', 'thepathoftruth.com')
;
insert into scriptures (name, translation_abbreviation)
values ('John', 'KJV')
     , ('John', 'MKJV');

select *
from scriptures

  
 */
