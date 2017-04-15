<?php

namespace Drupal\msdpp\Plugin\Block;

use Drupal\Core\Block\BlockBase;
use Drupal\Core\Session\AccountInterface;
use Drupal\Core\Block\BlockPluginInterface;
use Drupal\Core\Form\FormStateInterface;
use Drupal\Component\Utility;

use Symfony\Component\HttpFoundation\Response;

/**
 * Documents block.
 *
 * @Block(
 *   id = "document_block",
 *   admin_label = @Translation("Document Block"),
 * )
 */
class DocumentBlock extends BlockBase implements BlockPluginInterface {

    public function build(){

        \Drupal::service('page_cache_kill_switch')->trigger();

        $default_config = \Drupal::config('msdpp.settings');  
        $azureuri = $default_config->get('azure.uri');
        $clientid = $default_config->get('azure.clientid');
        $secret   = $default_config->get('azure.secret');
        $userid = \Drupal::currentUser()->id();

        $endpoint = rtrim($azureuri,'/') . '/Modules/Document?t='. mt_rand() . '&clientid=' . $clientid . '&secret=' . $secret . '&userid=' . $userid;

        $content = 'There was an error loading the module.'; 

        if(filter_var($endpoint, FILTER_VALIDATE_URL))  
        {
            $content = file_get_contents($endpoint);                 
        }

        $element = array(
        '#theme' => 'msdpp_formatter',
        '#msdpp_result' => $content,
        '#cache' => [ 'max-age' => 1 ]
        );

        return $element;

    }

    public function access_block(AccountInterface $account) {
        return $account->hasPermission("access content");
    }
}
