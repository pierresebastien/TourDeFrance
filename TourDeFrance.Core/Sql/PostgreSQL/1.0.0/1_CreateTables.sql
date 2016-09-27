CREATE TABLE "versions" 
(
  "major" integer,
  "minor" integer,
  "patch" integer,
  PRIMARY KEY("major", "minor", "patch")
); 

CREATE TABLE "config" 
(
  "key" citext NOT NULL, 
  "value" text NULL, 
  "default_value" text NULL, 
  "display_name" text NULL, 
  "description" text NULL, 
  "validation_regex" text NULL, 
  "type" text NOT NULL, 
  "order" integer NOT NULL, 
  "dangerous" boolean NOT NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL 
); 

CREATE UNIQUE INDEX uidx_config_key ON "config" ("key" ASC); 

CREATE TABLE "users" 
(
  "username" citext NOT NULL, 
  "password" text NULL, 
  "salt" text NULL, 
  "first_name" citext NULL, 
  "last_name" citext NULL, 
  "gender" text NOT NULL, 
  "birth_date" timestamp NULL, 
  "email" citext NULL, 
  "is_administrator" boolean NOT NULL, 
  "is_blocked" boolean NOT NULL, 
  "is_disabled" boolean NOT NULL, 
  "last_password_change_date" timestamp NOT NULL, 
  "previous_passwords" text NULL, 
  "require_new_password_at_logon" boolean NOT NULL, 
  "number_of_failed_attempts" integer NOT NULL, 
  "api_key" citext NULL,
  "height" numeric(38,6) NULL,
  "weight" numeric(38,6) NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL 
); 

CREATE UNIQUE INDEX uidx_users_username ON "users" ("username" ASC); 

CREATE TABLE "global_mail_templates" 
(
  "template" text NULL, 
  "name" citext NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL 
); 

CREATE UNIQUE INDEX uidx_global_mail_templates_name ON "global_mail_templates" ("name" ASC); 

CREATE TABLE "mail_templates" 
(
  "global_mail_template_id" uuid NOT NULL, 
  "default_mail_template_id" uuid NULL, 
  "type" text NULL, 
  "subject_template" text NULL, 
  "title_template" text NULL, 
  "sub_title_template" text NULL, 
  "body_template" text NULL, 
  "footer_template" text NULL, 
  "number_of_override" integer NULL, 
  "owner" uuid NOT NULL, 
  "name" citext NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_mail_templates_global_mail_templates_global_mail_template_id" FOREIGN KEY ("global_mail_template_id") REFERENCES "global_mail_templates" ("id"), 
  CONSTRAINT "FK_mail_templates_mail_templates_default_mail_template_id" FOREIGN KEY ("default_mail_template_id") REFERENCES "mail_templates" ("id"), 
  CONSTRAINT "FK_mail_templates_users_owner" FOREIGN KEY ("owner") REFERENCES "users" ("id") 
);

CREATE UNIQUE INDEX uidx_mail_templates_name ON "mail_templates" ("name" ASC); 

CREATE TABLE "drinks" 
(
  "alcohol_by_volume" numeric(38,6) NULL,
  "volume" numeric(38,6) NULL, 
  "owner" uuid NOT NULL, 
  "name" citext NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_drinks_users_owner" FOREIGN KEY ("owner") REFERENCES "users" ("id") 
); 

CREATE UNIQUE INDEX uidx_drinks_name ON "drinks" ("name" ASC); 

CREATE TABLE "sub_drinks" 
(
  "drink_id" uuid NOT NULL,
  "sub_drink_id" uuid NOT NULL, 
  "volume" numeric(38,6) NOT NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_sub_drinks_drinks_drink_id" FOREIGN KEY ("drink_id") REFERENCES "drinks" ("id"),
  CONSTRAINT "FK_sub_drinks_drinks_sub_drink_id" FOREIGN KEY ("sub_drink_id") REFERENCES "drinks" ("id")
); 

CREATE TABLE "stages" 
(
  "duration" integer NOT NULL, 
  "owner" uuid NOT NULL, 
  "name" citext NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_stages_users_owner" FOREIGN KEY ("owner") REFERENCES "users" ("id") 
);

CREATE UNIQUE INDEX uidx_stages_name ON "stages" ("name" ASC); 

CREATE TABLE "stage_drinks" 
(
  "stage_id" uuid NOT NULL, 
  "drink_id" uuid NOT NULL, 
  "order" integer NOT NULL,
  "overrided_volume" numeric(38,6) NULL,
  "number_to_drink" integer NOT NULL,
  "type" text NULL,
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_stage_drinks_stages_stage_id" FOREIGN KEY ("stage_id") REFERENCES "stages" ("id"), 
  CONSTRAINT "FK_stage_drinks_races_race_id" FOREIGN KEY ("drink_id") REFERENCES "drinks" ("id") 
); 

CREATE TABLE "races" 
(
  "owner" uuid NOT NULL, 
  "name" citext NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 

  CONSTRAINT "FK_races_users_owner" FOREIGN KEY ("owner") REFERENCES "users" ("id") 
); 

CREATE UNIQUE INDEX uidx_races_name ON "races" ("name" ASC); 

CREATE TABLE "race_stages" 
(
  "stage_id" uuid NOT NULL, 
  "race_id" uuid NOT NULL, 
  "order" integer NOT NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_race_stages_stages_stage_id" FOREIGN KEY ("stage_id") REFERENCES "stages" ("id"), 
  CONSTRAINT "FK_race_stages_races_race_id" FOREIGN KEY ("race_id") REFERENCES "races" ("id") 
); 

CREATE TABLE "teams" 
(
  "logo" bytea NULL, 
  "owner" uuid NOT NULL, 
  "name" citext NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 

  CONSTRAINT "FK_teams_users_owner" FOREIGN KEY ("owner") REFERENCES "users" ("id") 
); 

CREATE UNIQUE INDEX uidx_teams_name ON "teams" ("name" ASC); 

CREATE TABLE "riders" 
(
  "first_name" citext NULL, 
  "last_name" citext NULL, 
  "gender" text NOT NULL, 
  "birth_date" timestamp NULL, 
  "nationality" text NULL, 
  "height" numeric(38,6) NULL, 
  "weight" numeric(38,6) NULL, 
  "picture" bytea NULL, 
  "team_id" uuid NOT NULL, 
  "owner" uuid NOT NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_riders_teams_team_id" FOREIGN KEY ("team_id") REFERENCES "teams" ("id"), 
  CONSTRAINT "FK_riders_users_owner" FOREIGN KEY ("owner") REFERENCES "users" ("id") 
); 

CREATE TABLE "games" 
(
  "race_id" uuid NOT NULL, 
  "status" text NOT NULL, 
  "is_strict" boolean NOT NULL, 
  "auto_pause" boolean NOT NULL, 
  "owner" uuid NOT NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_games_races_race_id" FOREIGN KEY ("race_id") REFERENCES "races" ("id"), 
  CONSTRAINT "FK_games_users_owner" FOREIGN KEY ("owner") REFERENCES "users" ("id") 
); 

CREATE TABLE "players" 
(
  "nickname" citext NULL, 
  "first_name" citext NULL, 
  "last_name" citext NULL, 
  "gender" text NOT NULL, 
  "birth_date" timestamp NULL, 
  "height" numeric(38,6) NULL,
  "weight" numeric(38,6) NULL, 
  "owner" uuid NOT NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_players_users_owner" FOREIGN KEY ("owner") REFERENCES "users" ("id") 
); 

CREATE UNIQUE INDEX uidx_players_nickname ON "players" ("nickname" ASC); 
CREATE UNIQUE INDEX uidx_players_first_name_last_name ON "players" ("first_name", "last_name" ASC); 

CREATE TABLE "game_participants" 
(
  "game_id" uuid NOT NULL, 
  "player_id" uuid NULL, 
  "user_id" uuid NULL, 
  "team_id" uuid NOT NULL, 
  "rider_id" uuid NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_game_participants_games_game_id" FOREIGN KEY ("game_id") REFERENCES "games" ("id"), 
  CONSTRAINT "FK_game_participants_players_player_id" FOREIGN KEY ("player_id") REFERENCES "players" ("id"),
  CONSTRAINT "FK_game_participants_users_user_id" FOREIGN KEY ("user_id") REFERENCES "users" ("id"), 
  CONSTRAINT "FK_game_participants_teams_team_id" FOREIGN KEY ("team_id") REFERENCES "teams" ("id"), 
  CONSTRAINT "FK_game_participants_riders_rider_id" FOREIGN KEY ("rider_id") REFERENCES "riders" ("id") 
); 

CREATE TABLE "logs" 
(
  "game_id" uuid NULL, 
  "user_id" uuid NULL, 
  "game_participant_id" uuid NULL, 
  "date" timestamp NOT NULL, 
  "type" text NOT NULL, 
  "parameters" text NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_logs_games_game_id" FOREIGN KEY ("game_id") REFERENCES "games" ("id"), 
  CONSTRAINT "FK_logs_users_user_id" FOREIGN KEY ("user_id") REFERENCES "users" ("id"), 
  CONSTRAINT "FK_logs_game_participants_game_participant_id" FOREIGN KEY ("game_participant_id") REFERENCES "game_participants" ("id") 
); 

CREATE TABLE "scores" 
(
  "game_participant_id" uuid NOT NULL, 
  "stage_id" uuid NOT NULL, 
  "drink_id" uuid NOT NULL, 
  "number" integer NOT NULL, 
  "id" uuid PRIMARY KEY, 
  "creation_date" timestamp NOT NULL, 
  "last_update_date" timestamp NOT NULL, 
  "last_update_by" text NULL, 
  CONSTRAINT "FK_scores_game_participants_game_participant_id" FOREIGN KEY ("game_participant_id") REFERENCES "game_participants" ("id"), 
  CONSTRAINT "FK_scores_stages_stage_id" FOREIGN KEY ("stage_id") REFERENCES "stages" ("id"), 
  CONSTRAINT "FK_scores_drinks_drink_id" FOREIGN KEY ("drink_id") REFERENCES "drinks" ("id") 
); 