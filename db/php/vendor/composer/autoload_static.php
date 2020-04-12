<?php

// autoload_static.php @generated by Composer

namespace Composer\Autoload;

class ComposerStaticInit1022399b75112eeb2d24ac417c48b3dd
{
    public static $prefixLengthsPsr4 = array (
        'G' => 
        array (
            'Games\\' => 6,
        ),
    );

    public static $prefixDirsPsr4 = array (
        'Games\\' => 
        array (
            0 => __DIR__ . '/../..' . '/games',
        ),
    );

    public static function getInitializer(ClassLoader $loader)
    {
        return \Closure::bind(function () use ($loader) {
            $loader->prefixLengthsPsr4 = ComposerStaticInit1022399b75112eeb2d24ac417c48b3dd::$prefixLengthsPsr4;
            $loader->prefixDirsPsr4 = ComposerStaticInit1022399b75112eeb2d24ac417c48b3dd::$prefixDirsPsr4;

        }, null, ClassLoader::class);
    }
}