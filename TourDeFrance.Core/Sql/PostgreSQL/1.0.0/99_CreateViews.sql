CREATE VIEW view_share_access AS
SELECT access_shares.sharing_user_id AS sharing_user_id,
       access_shares.shared_user_id AS shared_user_id,
       access_shares.id AS id,
       access_shares.creation_date AS creation_date,
       access_shares.last_update_date AS last_update_date,
       access_shares.last_update_by AS last_update_by,
       u1.first_name AS sharing_first_name,
       u1.last_name AS sharing_last_name,
       u1.username AS sharing_username,
       u2.first_name AS shared_first_name,
       u2.last_name AS shared_last_name,
       u2.username AS shared_username
FROM access_shares 
INNER JOIN users u1 ON access_shares.sharing_user_id = u1.id
INNER JOIN users u2 ON access_shares.shared_user_id = u2.id
WHERE u1.is_disabled = false
   AND u2.is_disabled = false;