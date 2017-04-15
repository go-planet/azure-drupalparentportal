<?php
/**
 * @file
 * @author Planet Technologies
 * Contains \Drupal\msdpp\Controller\MsdppController.
 * Please place this file under your msdpp(module_root_folder)/src/Controller/
 */
namespace Drupal\msdpp\Controller;

use Drupal\Core\Controller\ControllerBase;
use Drupal\Core\Config;
use Symfony\Component\HttpFoundation\Response;

/**
 * Provides route responses for the Msdpp module. TODO: Check URI value
 */
class MsdppController extends ControllerBase  {

    public function getevents() {

        $config = new MsdppConfiguration();

        $response = new Response();
        $response->setContent('[]');
        $result = file_get_contents($config->uri() . 'modules/getevents?clientid=' . $config->clientid() . '&secret=' . $config->secret() . '&userid=' . $config->userid() . '&start=2016-01-01&end=2018-01-01');       
        $response->setContent($result);
        $response->headers->set('Content-Type', 'application/json');
        return $response; 

   }

    public function isuservolunteer() {

        $config = new MsdppConfiguration();

        $response = new Response();
        $response->setContent('[]');

        if(isset($_GET['eventid'])) 
        {
            $eventid = $_GET['eventid'];
            $result = file_get_contents($config->uri() . 'modules/isuservolunteer?clientid=' . $config->clientid() . '&secret=' . $config->secret() . '&eventid=' . $eventid . '&userid=' . $config->userid());       
            $response->setContent($result);
            $response->headers->set('Content-Type', 'application/json');
        }

        return $response; 

   }

   public function addvolunteer() {

        $config = new MsdppConfiguration();

        $response = new Response();
        $response->setContent('[]');

        if(isset($_POST["values"])) 
        {
            $values = $_POST["values"];

            $postdata = http_build_query(
                    array(
                        'values' => $values,
                        'clientid' => $config->clientid(),
                        'secret' => $config->secret(), 
                        'userid' => $config->userid()
                    )
                );
                
            $opts = array('http' =>
                    array(
                        'method'  => 'POST',
                        'header'  => 'Content-type: application/x-www-form-urlencoded',
                        'content' => $postdata
                    )
                );

            $context  = stream_context_create($opts);
            $result = file_get_contents($config->uri() . 'modules/addvolunteer', false, $context);       
            $response->setContent($result);
        } //endif

        $response->headers->set('Content-Type', 'application/json');
        return $response; 

    }

    public function unvolunteer() {

        $config = new MsdppConfiguration();

        $response = new Response();
        $response->setContent('[]');

        if(isset($_POST["eventid"])) 
        {
            $eventid = $_POST["eventid"];

            $postdata = http_build_query(
                    array(
                        'eventid' => $eventid,
                        'clientid' => $config->clientid(),
                        'secret' => $config->secret(), 
                        'userid' => $config->userid()
                    )
                );
                
            $opts = array('http' =>
                    array(
                        'method'  => 'POST',
                        'header'  => 'Content-type: application/x-www-form-urlencoded',
                        'content' => $postdata
                    )
                );

            $context  = stream_context_create($opts);
            $result = file_get_contents($config->uri() . 'modules/unvolunteer', false, $context);       
            $response->setContent($result);
        } //endif

        $response->headers->set('Content-Type', 'application/json');
        return $response; 

    }

    public function adminurl() {
        $config = new MsdppConfiguration();
        $response = new Response();
        $response->setContent($config->uri());
        return $response;     
    }

    public function download() {

        $config = new MsdppConfiguration();

        $response = new Response();
        $response->setContent('');

        if(isset($_GET['documentid'])) 
        {
            $documentid = $_GET['documentid'];
            $documentname = $_GET['documentname'];
            $result = file_get_contents($config->uri() . 'modules/download?clientid=' . $config->clientid() . '&secret=' . $config->secret() . '&documentid=' . $documentid . '&userid=' . $config->userid());       
            $response->setContent($result);
            $response->headers->set('Content-Type', 'application/octet-stream');
            $response->headers->set('Content-Disposition', 'attachment;filename=' . $documentname);
            $response->headers->set('Content-Length', '');
            
        }
                
        return $response; 
   }
        
    public function getdocumentcount() {

        $config = new MsdppConfiguration();

        $response = new Response();
        $response->setContent('[]');

        $result = file_get_contents($config->uri() . 'modules/getdocumentcount?clientid=' . $config->clientid() . '&secret=' . $config->secret() . '&userid=' . $config->userid());       
        $response->setContent($result);
        $response->headers->set('Content-Type', 'application/json');

        return $response; 
   }

    public function getdocuments() {

        $config = new MsdppConfiguration();

        $response = new Response();
        $response->setContent('[]');

        if(isset($_GET['page'])) 
        {
            $page = $_GET['page'];
            $result = file_get_contents($config->uri() . 'modules/documents?clientid=' . $config->clientid() . '&secret=' . $config->secret() . '&page=' . $page . '&userid=' . $config->userid());       
            $response->setContent($result);
        }

        $response->headers->set('Content-Type', 'application/json');
        return $response; 
   }

   public function savestudent() {
        
        $config = new MsdppConfiguration();

        $response = new Response();
        $response->setContent('[]');

        if(isset($_POST["values"])) 
        {
            $values = $_POST["values"];

            $postdata = http_build_query(
                    array(
                        'values' => $values,
                        'clientid' => $config->clientid(),
                        'secret' => $config->secret(), 
                        'userid' => $config->userid()
                    )
                );
                
            $opts = array('http' =>
                    array(
                        'method'  => 'POST',
                        'header'  => 'Content-type: application/x-www-form-urlencoded',
                        'content' => $postdata
                    )
                );

            $context  = stream_context_create($opts);
            $result = file_get_contents($config->uri() . 'modules/savestudent', false, $context);       
            $response->setContent($result);
        } //endif

        $response->headers->set('Content-Type', 'application/json');
        return $response; 
    }

    public function removestudent() {
        
        $config = new MsdppConfiguration();

        $response = new Response();
        $response->setContent('[]');

        if(isset($_POST["studentid"])) 
        {
            $studentid = $_POST["studentid"];

            $postdata = http_build_query(
                    array(
                        'studentid' => $studentid,
                        'clientid' => $config->clientid(),
                        'secret' => $config->secret(), 
                        'userid' => $config->userid()
                    )
                );
                
            $opts = array('http' =>
                    array(
                        'method'  => 'POST',
                        'header'  => 'Content-type: application/x-www-form-urlencoded',
                        'content' => $postdata
                    )
                );

            $context  = stream_context_create($opts);
            $result = file_get_contents($config->uri() . 'modules/removestudent', false, $context);       
            $response->setContent($result);
        } //endif

        $response->headers->set('Content-Type', 'application/json');
        return $response; 
    }



}


class MsdppConfiguration
{
    public function uri() {
        $default_config = \Drupal::config('msdpp.settings');  
        return $default_config->get('azure.uri');
    }

    public function clientid() {
        $default_config = \Drupal::config('msdpp.settings');  
        return $default_config->get('azure.clientid');
    }
    
    public function secret() {
        $default_config = \Drupal::config('msdpp.settings');  
        return $default_config->get('azure.secret');
    }

    public function userid() {
        return \Drupal::currentUser()->id();
    }

}


?>