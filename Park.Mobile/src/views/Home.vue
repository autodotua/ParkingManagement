<template>
  <div>
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>我的资金</span>
        <el-button style="float: right; padding: 3px 0" type="text">交易记录</el-button>
      </div>
      <el-row>
        <el-col :span="12">
          余额：
        </el-col>
        <el-col :span="8">
          {{displayBalance}}
        </el-col>
        <el-col :span="4">
          <el-button size="mini">充值</el-button>
        </el-col>
      </el-row>
      <br>
      <el-row>
        <el-col :span="12">
          月租到期时间：
        </el-col>
        <el-col :span="8">
          {{displayExpireTime}}
        </el-col>
        <el-col :span="4">
          <el-button size="mini">续期</el-button>
        </el-col>
      </el-row>
    </el-card>
    <br/>
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>我的车辆</span>
        <el-button style="float: right; padding: 3px 0" type="text">管理</el-button>
      </div>
      <el-table :data="cars" style="width: 100%">
        <el-table-column prop="licensePlate" label="车牌" width="120"></el-table-column>
        <el-table-column prop="records" label="停车次数" width="80"></el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script>
import Vue from "vue";
import Cookies from "js-cookie";
import { withToken } from "../common";
export default Vue.extend({
  name: "Home",
  data() {
    return {
      cars: [],
      balance:0,
      expireTime:""
    };
  },
  computed:{
    displayBalance(){
      return this.balance+"元";
    },
    displayExpireTime(){
      if(this.expireTime.startsWith("0001"))
      {
        return "无";
      }
      return this.expireTime;
    }
  },
  components: {},
  mounted: function() {
    this.$nextTick(function() {
      const userID = Cookies.get("userID") || "";
      Vue.axios
        .post("http://localhost:8520/User/Home", withToken({}))
        .then(response => {
          this.cars = response.data.data.cars;
          this.balance = response.data.data.balance;
          this.expireTime= response.data.data.expireTime;
          console.log(response.data);
        })
        .catch(r => {
          console.log(r.Message);
        });
    });
  }
});
</script>
