drop procedure if exists prune_logs;
DELIMITER ^_^
create procedure prune_logs(
    min_days_old int
)
BEGIN

    set @min_days_old = coalesce(min_days_old, 30); # if null, set a default.

    select @min_days_old, min_days_old as 'input';

    select exception_message, log_ages.days_old
    from (select id, TIMESTAMPDIFF(DAY, created_at, CURDATE()) AS days_old
          from logs
#logging
#.net
#.net-core

          order by days_old desc) as log_ages
             join logs log on log_ages.id = log.id
    where log_ages.days_old >= @min_days_old
       or log.exception_message is null;

end ^_^;
delimiter ;

/*TESTS*/
call prune_logs(20); 

# select * from logs order