CREATE TABLE `products` (
  `id` VARCHAR(80) NOT NULL
)

CHARACTER SET 'utf8mb4'
COLLATE 'utf8mb4_general_ci';

ALTER TABLE `products`
ADD INDEX `id` (`id`);