/** Sample inserts
insert into scriptures (name)
values ('foo'),
       ('bar'),
       ('baz');

insert into scriptures (name)
values ('Judges'),
       ('Revelation'),
       ('Galatians');
*/


select *
from scriptures
order by modified_at desc;


### Preventing duplicates

SET
    @name = '(Luke 6:56-57 GW).',
    @text =
            'Jesus said: “Those who eat My flesh and drink My blood live in Me, and I live in them. The Father Who has life sent Me, and I live because of the Father. So those who feed on Me will live because of Me”'
;

INSERT INTO scriptures
    (name, text)
VALUES (@name, @text)
ON DUPLICATE KEY UPDATE name = @name,
                        text = @text;

select *
from scriptures
order by modified_at desc;

# 
# select count(id)
# from scriptures;

# 
# delete from scriptures where id >= 1;

/*
 */
INSERT INTO scriptures (name, translation_abbreviation)
SELECT SUM(name),
       SUM(translation_abbreviation)
FROM scriptures
GROUP BY name, translation_abbreviation
ON DUPLICATE KEY UPDATE name=SUM(Name);


### delete duplicate scriptures
delete tbl_outer
FROM scriptures tbl_outer
         INNER JOIN scriptures tbl_inner
                    ON tbl_outer.Name = tbl_inner.Name
                        AND tbl_outer.translation = tbl_inner.translation;

select name, translation, translation_abbreviation from scriptures;

select distinct name, translation
from scriptures;



SELECT t1.Name, t1.Text#, t1.last_name, t1.film_id, t1.title, t1.rental_date
FROM scriptures t1
WHERE EXISTS(
              SELECT 1
              FROM scriptures t2
              WHERE t1.Name = t2.Name
                AND t1.Text = t2.Text
          #                 AND t1.rental_date = t2.rental_date
#                 AND t1.rental_id != t2.rental_id
          )
#     as 'duplicate scriptures'
;



## Find duplicates (id only for now...)
## from: https://blog.devart.com/delete-duplicate-rows-in-mysql.html
SELECT id,
       name,
       text,
       COUNT(id)
FROM scriptures
GROUP BY id
HAVING COUNT(id) > 1;

## count instances
SELECT Name,
       ROW_NUMBER() OVER (
           PARTITION BY Name
           ORDER BY Name
           ) AS counts
FROM scriptures;