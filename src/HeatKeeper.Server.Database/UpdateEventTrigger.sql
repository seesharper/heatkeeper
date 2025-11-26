update EventTriggers 
   set Name = @name,
       Definition = @definition
 where Id = @id;