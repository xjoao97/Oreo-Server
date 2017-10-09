-- phpMyAdmin SQL Dump
-- version 4.7.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: 10-Out-2017 às 00:56
-- Versão do servidor: 10.1.25-MariaDB
-- PHP Version: 5.6.31

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `quasar`
--

-- --------------------------------------------------------

--
-- Estrutura da tabela `permissions_commands`
--

CREATE TABLE `permissions_commands` (
  `command` varchar(45) NOT NULL DEFAULT '',
  `group_id` int(11) NOT NULL DEFAULT '4',
  `subscription_id` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Extraindo dados da tabela `permissions_commands`
--

INSERT INTO `permissions_commands` (`command`, `group_id`, `subscription_id`) VALUES
('command_about', 1, 0),
('command_addpredesigned', 9, 0),
('command_addtags', 9, 0),
('command_alerttype', 9, 0),
('command_alert_user', 3, 0),
('command_allaroundme', 3, 0),
('command_alleyesonme', 3, 0),
('command_ban', 5, 0),
('command_bubble', 5, 0),
('command_carry', 1, 0),
('command_clubnx', 5, 0),
('command_commands', 1, 0),
('command_control', 1, 0),
('command_convert_credits', 5, 0),
('command_coords', 5, 0),
('command_dance', 1, 0),
('command_debug', 8, 0),
('command_delete_group', 5, 0),
('command_developer', 9, 0),
('command_disable_diagonal', 1, 0),
('command_disable_gifts', 1, 0),
('command_disable_mimic', 1, 0),
('command_disconnect', 5, 0),
('command_djalert', 8, 0),
('command_dnd', 1, 0),
('command_ejectall', 1, 0),
('command_empty', 1, 0),
('command_empty_items', 1, 0),
('command_enable', 1, 0),
('command_enable_friends', 1, 0),
('command_eventlist', 1, 0),
('command_event_alert', 4, 0),
('command_faceless', 1, 0),
('command_fastwalk', 1, 0),
('command_flagme', 5, 0),
('command_flaguser', 5, 0),
('command_follow', 1, 0),
('command_forced_effects', 5, 0),
('command_forcesit', 5, 0),
('command_force_draw', 5, 0),
('command_freeze', 5, 0),
('command_give', 5, 0),
('command_give_badge', 5, 0),
('command_give_coins', 5, 0),
('command_give_diamonds', 5, 0),
('command_give_gotw', 5, 0),
('command_give_pixels', 5, 0),
('command_give_rank', 8, 0),
('command_give_special', 5, 0),
('command_golpe', 1, 0),
('command_goto', 1, 0),
('command_groepchat', 1, 0),
('command_guide_alert', 4, 0),
('command_ha', 9, 0),
('command_habnam', 1, 0),
('command_hal', 5, 0),
('command_hotel_alert', 5, 0),
('command_hvusers', 5, 0),
('command_ignore_whispers', 1, 0),
('command_ip_ban', 5, 0),
('command_kick', 5, 0),
('command_kickbots', 1, 0),
('command_kickpets', 1, 0),
('command_lay', 1, 0),
('command_maintenance', 8, 0),
('command_makesay', 8, 0),
('command_makevip', 8, 0),
('command_make_say', 8, 0),
('command_make_shout', 1, 0),
('command_massdance', 8, 0),
('command_massenable', 8, 0),
('command_massevent', 8, 0),
('command_massgive', 8, 0),
('command_mass_badge', 8, 0),
('command_mimic', 1, 1),
('command_mip', 5, 0),
('command_moonwalk', 1, 0),
('command_mute', 4, 0),
('command_mute_bots', 1, 0),
('command_mute_pets', 1, 0),
('command_override', 8, 0),
('command_override_massenable', 5, 0),
('command_pet', 1, 0),
('command_pickall', 1, 0),
('command_prefix', 1, 0),
('command_publi_alert', 8, 0),
('command_pull', 1, 0),
('command_push', 1, 0),
('command_regen_maps', 1, 0),
('command_reload', 1, 0),
('command_removepredesigned', 8, 0),
('command_rig', 1, 0),
('command_room', 1, 0),
('command_roommute', 5, 0),
('command_room_alert', 5, 0),
('command_room_badge', 5, 0),
('command_room_kick', 5, 0),
('command_room_say', 5, 0),
('command_room_shout', 5, 0),
('command_sell_room', 9, 0),
('command_setbet', 1, 0),
('command_setmax', 1, 0),
('command_setspeed', 1, 0),
('command_shoot', 1, 0),
('command_sit', 1, 0),
('command_staffinfo', 5, 0),
('command_staff_alert', 4, 0),
('command_stand', 1, 0),
('command_stats', 1, 0),
('command_summon', 4, 0),
('command_summonall', 5, 0),
('command_super_fastwalk', 5, 0),
('command_super_pull', 1, 1),
('command_super_push', 1, 1),
('command_teleport', 4, 0),
('command_trade_ban', 4, 0),
('command_transfer', 4, 0),
('command_unfreeze', 4, 0),
('command_unload', 1, 0),
('command_unmute', 4, 0),
('command_unroommute', 4, 0),
('command_update', 8, 0),
('command_update_achievements', 8, 0),
('command_update_anti_mutant', 8, 0),
('command_update_bans', 5, 0),
('command_update_bots', 5, 0),
('command_update_catalog', 8, 0),
('command_update_cata_full', 8, 0),
('command_update_chat_styles', 5, 0),
('command_update_configuration', 8, 0),
('command_update_filter', 4, 0),
('command_update_furni', 8, 0),
('command_update_game_center', 8, 0),
('command_update_models', 8, 0),
('command_update_moderation', 8, 0),
('command_update_navigator', 5, 0),
('command_update_pinatas', 8, 0),
('command_update_promotions', 8, 0),
('command_update_quests', 8, 0),
('command_update_rewards', 8, 0),
('command_update_rights', 8, 0),
('command_update_tickets', 8, 0),
('command_update_vouchers', 8, 0),
('command_update_youtube', 8, 0),
('command_user_info', 5, 0),
('command_viewinventary', 9, 0),
('command_voucher', 9, 0),
('command_yyxxabxa', 1, 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `permissions_commands`
--
ALTER TABLE `permissions_commands`
  ADD PRIMARY KEY (`command`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
